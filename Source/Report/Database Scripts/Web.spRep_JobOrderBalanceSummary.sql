USE [RUG]
GO
/****** Object:  StoredProcedure [Web].[spRep_JobOrderBalanceSummary]    Script Date: 16/Jan/2016 11:49:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [Web].[spRep_JobOrderBalanceSummary] 	
    @Site VARCHAR(20) = NULL,
    @Division VARCHAR(20) = NULL,
	@FromDate VARCHAR(20) = NULL,
	@ToDate VARCHAR(20) = NULL,
	@DocumentType VARCHAR(20) = NULL,	
    @JobWorker VARCHAR(Max) = NULL,
    @Process VARCHAR(Max) = NULL,
    @ProductNature VARCHAR(Max) = NULL,
    @ProductType VARCHAR(Max) = NULL,
    @ProductGroup VARCHAR(Max) = NULL,
    @ProductCustomGroup VARCHAR(Max) = NULL,
    @PersonCustomGroup VARCHAR(Max) = NULL,
    @Product VARCHAR(Max) = NULL,     
    @ProdOrderHeaderId VARCHAR(Max) = NULL,  
	@JobOrderHeaderId VARCHAR(Max) = NULL,
	@ProductDivisionId VARCHAR(Max)=NULL,
	@StatusOnDate VARCHAR(20)=NULL,
	@ReportType VARCHAR(200)=NULL,
	@ProductCategory VARCHAR(max)=NULL
	
AS
BEGIN 


SET @StatusOnDate =(CASE WHEN @StatusOnDate IS NULL THEN getdate() ELSE @StatusOnDate END )

SELECT 

H.ProductcategoryName AS ProductCategoryName,
 H.Date  AS Date,
Max(H.Months) AS Months ,
H.SiteId,
H.DivisionId,

max(H.SiteName) AS SiteName,
max(H.DivisionName) AS DivisionName,
Sum(isnull(H.BalanceQty,0)) AS BalanceQty,
 Sum(Isnull(H.Area,0)) AS Area,
Max(H.UnitName) AS UnitName,
IsNull(max(H.DecimalPlaces),0) AS DecimalPlaces,
(CASE WHEN @ReportType='PCS Wise Summary' THEN 'JobOrderBalancePCSWiseSummary.rdl'
  WHEN @ReportType='PCS With Area Wise Summary' THEN 'JobOrderBalancePCSWithAreaWiseSummary.rdl'
  ELSE 'JobOrderBalanceAreaWiseSummary.rdl'  END)  AS ReportName, 
'Loom Balance Summary' AS ReportTitle, NULL AS SubReportProcList

FROM (
SELECT 
PC.ProductCategoryName AS ProductcategoryName,
Substring(convert(NVARCHAR,H.OrderDate,11),0,6) AS Date,
Max(STUFF(CONVERT(CHAR(11), H.OrderDate, 100), 5,5, '-')) AS Months ,
H.SiteId ,
H.DivisionId,
max(S.SiteCode) AS SiteName,
max(D.DivisionName) AS DivisionName,
Sum(isnull(H.BalanceQty,0)) AS BalanceQty,
(SELECT * FROM Web.FuncConvertSqFeetToSqYard(max(Case When H.UnitconversionForId='5' THEN VRS.ManufaturingSizeArea When H.UnitconversionForId='4' THEN VRS.FinishingSizeArea else VRS.StandardSizeArea END)))* Sum(isnull(H.BalanceQty,0)) AS Area,

Max(U.UnitName) AS UnitName,
IsNull(max(U.DecimalPlaces),0) AS DecimalPlaces

FROM Web.FJobOrderBalance(@StatusOnDate, @Site,@Division, @FromDate, @ToDate, @DocumentType, @JobWorker,@Process,@Product, @JobOrderHeaderId) AS H 
LEFT JOIN Web.Products P ON H.ProductId = P.ProductId
LEFT Join Web.FinishedProduct FP WITH (Nolock) ON FP.ProductId=P.ProductId 
LEFT JOIN Web.ProductCategories PC WITH (Nolock) ON FP.ProductCategoryId=PC.ProductCategoryId
LEFT JOIN Web.ProductGroups Pg ON P.ProductGroupId = Pg.ProductGroupId
LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId
LEFT JOIN web.ViewRugSize VRS with (nolock) on VRS.ProductId=P.ProductId
LEFT JOIN Web.Units U ON P.UnitId = U.UnitId
LEFT JOIN Web._People Pp ON H.JobWorkerId = Pp.PersonID 
LEFT JOIN Web.ProdOrderLines Pil ON H.ProdOrderLineId = Pil.ProdOrderLineId
LEFT JOIN web.Processes PR ON PR.ProcessId = H.ProcessId
LEFT JOIN Web.Sites S ON H.SiteId = S.SiteId
LEFT JOIN Web.Divisions D ON H.DivisionId = D.DivisionId

LEFT JOIN (
			SELECT ProductId FROM Web.ProductCustomGroupLines
			WHERE 1=1 
			AND ( @ProductCustomGroup is null or ProductCustomGroupHeaderId IN (SELECT Items FROM [dbo].[Split] (@ProductCustomGroup, ','))) 
			GROUP BY ProductId
			) AS PCG  ON  H.ProductId =PCG.ProductId
WHERE 1=1 AND Pt.ProductTypeId = 1 
AND ( @ProductCategory is null or FP.ProductCategoryId IN (SELECT Items FROM [dbo].[Split] (@ProductCategory, ','))) 
AND ( @ProductDivisionId is null or P.DivisionId IN (SELECT Items FROM [dbo].[Split] (@ProductDivisionId, ',')))  
AND ( @ProdOrderHeaderId IS NULL OR Pil.ProdOrderHeaderId IN (SELECT Items FROM [dbo].[Split] (@ProdOrderHeaderId, ','))) 
AND ( @ProductGroup IS NULL OR Pg.ProductGroupId IN (SELECT Items FROM [dbo].[Split] (@ProductGroup, ','))) 
AND ( @ProductType IS NULL OR Pt.ProductTypeId IN (SELECT Items FROM [dbo].[Split] (@ProductType, ','))) 
AND ( @ProductNature IS NULL OR Pt.ProductNatureId IN (SELECT Items FROM [dbo].[Split] (@ProductNature, ','))) 
AND (@ProductCustomGroup IS NULL OR PCG.ProductId IS NOT NULL)
AND ( @PersonCustomGroup IS NULL OR H.JobWorkerId IN (SELECT PersonId FROM Web.PersonCustomGroupLines WHERE PersonCustomGroupHeaderId IN (SELECT Items FROM [dbo].[Split] (@PersonCustomGroup, ',')))) 
GROUP BY PC.ProductCategoryName,Substring(convert(NVARCHAR,H.OrderDate,11),0,6),H.SiteId ,H.DivisionId,H.ProductId
--ORDER BY PC.ProductCategoryName,Substring(convert(NVARCHAR,H.OrderDate,11),0,6),H.SiteId ,H.DivisionId,H.ProductId
) AS H
GROUP BY H.ProductcategoryName,H.Date,H.SiteId,H.DivisionId
ORDER BY H.Date,H.ProductcategoryName,H.SiteId,H.DivisionId
End