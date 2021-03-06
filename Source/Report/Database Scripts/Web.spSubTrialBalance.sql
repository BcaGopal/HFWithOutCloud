USE [RUG]
GO
/****** Object:  StoredProcedure [Web].[spSubTrialBalance]    Script Date: 05/Aug/2015 11:06:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [Web].[spSubTrialBalance]     
    @Site VARCHAR(20) = NULL,
    @Division VARCHAR(20) = NULL,
	@AsOnDate VARCHAR(20) = NULL,
    @LedgerAccountGroupId VARCHAR(20) = NULL
AS
BEGIN

declare @DivisionCnt Int
declare @SiteCnt Int
set @DivisionCnt='0'
set @SiteCnt='0'

Declare @SiteText varchar(max)
set @SiteText=(Select SiteName + ',' from Web.Sites where SiteId In (SELECT Items FROM [dbo].[Split] (@Site, ',')) FOR XML path (''))
set @SiteText=LEFT(@SiteText,len(@SiteText)-1)

declare @DivisionText varchar(max)
set @DivisionText=(Select DivisionName + ',' from web.Divisions where DivisionId In (SELECT Items FROM [dbo].[Split] (@Division, ',')) FOR XML path (''))
set @DivisionText=LEFT(@DivisionText,len(@DivisionText)-1)

Declare @LedgerAccountGroupText varchar(Max)
set  @LedgerAccountGroupText=(Select LedgerAccountGroupName + ',' from web.LedgerAccountGroups where LedgerAccountGroupId In (SELECT Items FROM [dbo].[Split] (@LedgerAccountGroupId, ',')) FOR XML path (''))
set @LedgerAccountGroupText=LEFT(@LedgerAccountGroupText,len(@LedgerAccountGroupText)-1)

/*count for the Site and Division */
Select  @DivisionCnt=count(Distinct H.DivisionId),@SiteCnt=count(Distinct H.siteId)
from 
(
SELECT 
max(LH.SiteId)  as SiteId,max(LH.DivisionId) as DivisionId
FROM web.Ledgers H  WITH (Nolock) 
LEFT JOIN web.LedgerHeaders LH WITH (Nolock) ON LH.LedgerHeaderId = H.LedgerHeaderId 
LEFT JOIN web.LedgerAccounts LA  WITH (Nolock) ON LA.LedgerAccountId = H.LedgerAccountId 
LEFT JOIN web.LedgerAccountGroups LAG  WITH (Nolock) ON LAG.LedgerAccountGroupId = LA.LedgerAccountGroupId 
WHERE LA.LedgerAccountId IS NOT NULL 
AND ( @Site is null or LH.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))) 
AND ( @Division is null or LH.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))) 
AND ( @AsOnDate is null or @AsOnDate >= LH.DocDate ) 
AND ( @LedgerAccountGroupId is null or LA.LedgerAccountGroupId IN (SELECT Items FROM [dbo].[Split] (@LedgerAccountGroupId, ','))) 
GROUP BY LA.LedgerAccountId 
HAVING sum(isnull(H.AmtDr,0)) <> 0 OR  sum(isnull(H.AmtCr,0)) <> 0
) as H


/*main query*/
SELECT LA.LedgerAccountId, max(LA.LedgerAccountName) AS LedgerAccountName, max(LAG.LedgerAccountGroupName) AS ReportTitle, 
CASE WHEN sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0)) > 0 THEN sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0)) ELSE NULL  END AS AmtDr,
CASE WHEN sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0)) < 0 THEN abs(sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0))) ELSE NULL END AS AmtCr,    
(case when @SiteCnt>1 THEN '0' else max(LH.SiteId) end) as SiteId,
(case when @DivisionCnt>1 THEN '0' else max(LH.DivisionId) end) as DivisionId,
'SubTrialBalance.rdl' AS ReportName, NULL AS SubReportProcList,@SiteText as Sitetext,@DivisionText as DivisionText,@LedgerAccountGroupText as LedgerAccountGroupText,@AsOnDate as OnDate
FROM web.Ledgers H  WITH (Nolock) 
LEFT JOIN web.LedgerHeaders LH WITH (Nolock) ON LH.LedgerHeaderId = H.LedgerHeaderId 
LEFT JOIN web.LedgerAccounts LA  WITH (Nolock) ON LA.LedgerAccountId = H.LedgerAccountId 
LEFT JOIN web.LedgerAccountGroups LAG  WITH (Nolock) ON LAG.LedgerAccountGroupId = LA.LedgerAccountGroupId 
WHERE LA.LedgerAccountId IS NOT NULL 
AND ( @Site is null or LH.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))) 
AND ( @Division is null or LH.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))) 
AND ( @AsOnDate is null or @AsOnDate >= LH.DocDate ) 
AND ( @LedgerAccountGroupId is null or LA.LedgerAccountGroupId IN (SELECT Items FROM [dbo].[Split] (@LedgerAccountGroupId, ','))) 
GROUP BY LA.LedgerAccountId 
HAVING sum(isnull(H.AmtDr,0)) <> 0 OR  sum(isnull(H.AmtCr,0)) <> 0
End

