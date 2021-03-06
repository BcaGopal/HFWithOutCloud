

IF OBJECT_ID ('[Web].[ProcSaleInvoicePrint]') IS NOT NULL
	DROP PROCEDURE [Web].[ProcSaleInvoicePrint]	
GO



CREATE PROCEDURE [Web].[ProcSaleInvoicePrint](@Id INT)  AS   BEGIN  
SELECT H.SaleInvoiceHeaderId, H.DocDate, H.DocNo, P.Name AS BillToBuyerName, VAddress.BillToBuyerAddress,  VAddress.BIllToPartyCity, VAddress.BillToPartyCountry,  C.Name AS CurrencyName,   Hd.BLNo, Hd.BLDate, Hd.PrivateMark, Hd.PortOfLoading, Hd.DestinationPort, Hd.FinalPlaceOfDelivery, Hd.PreCarriageBy,   Hd.PlaceOfPreCarriage, Hd.CircularNo, Hd.CircularDate, Hd.OrderNo, Hd.OrderDate, 
(SELECT dbo.SplitString(Hd.BaleNoSeries, ',',90)) AS BaleNoSeries,
Hd.DescriptionOfGoods, 
Replace(Replace(Hd.DescriptionOfGoods,CHAR(13),''),CHAR(10),' ') AS DescriptionOfGoodsWithoutCRLF ,  
 Hd.PackingMaterialDescription, Hd.KindsOfackages,   Hd.Compositions, Hd.OtherRefrence, Hd.TermsOfSale, Hd.NotifyParty, Hd.TransporterInformation,   Sm.ShipMethodName, Dt.DeliveryTermsName,   Prod.ProductName AS ProductName, Prod.ProductSpecification, Pg.ProductGroupName AS ProductDesignName, PCol.ColourName AS ProductColourName,   Pt.ProductTypeName, 
VProductArea.SizeName AS ProductSizeName, 
Pig.ProductInvoiceGroupName,   Soh.DocNo AS SaleOrderNo,  Pig.ItcHsCode, Pig.Knots, Pl.BaleNo, 
Try_Parse(Replace(Pl.BaleNo,'-','.') AS DECIMAL) AS BaleNoToSort, Pl.Qty, L.Rate, 
--Pl.Qty * IsNull(VProductArea.SqFeetArea,0) * L.Rate AS Amount,
Hd.InvoiceAmount,
L.Amount,   
CASE WHEN Pig.SeparateWeightInInvoice <> 0 THEN Pig.ProductInvoiceGroupName ELSE 'OTHER GROSS WEIGHT & NET WEIGHT' END AS WeightText,
CASE WHEN Pig.SeparateWeightInInvoice = 0 THEN 0 ELSE 1 END  SeparateWeightInInvoice,
IsNull(Hd.Freight,0) AS Freight,
VCustomDetail.CourierName, 
Hd.VehicleNo,
Du.UnitName AS DealUnitName,
VSaleInvoice.TotalDealQty AS TotalSqFeetArea,
CASE WHEN Pt.ProductTypeName = 'Rug' THEN Pl.DealQty ELSE 0 END AS SqFeetArea, 
--Pl.Qty * IsNull(VProductArea.SqFeetArea,0) AS SqFeetArea, 
Pl.Qty * IsNull(VProductArea.SqMeterArea,0) AS SqMeterArea  ,  Pl.GrossWeight, Pl.NetWeight,  
VSaleInvoice.TotalRugGrossWeight,VSaleInvoice.TotalFinishedProductGrossWeight,  VSaleInvoice.TotalGrossWeight,VSaleInvoice.TotalRugNetWeight,VSaleInvoice.TotalFinishedProductNetWeight,  VSaleInvoice.TotalNetWeight,VSaleInvoice.TotalRugQty,VSaleInvoice.TotalFinishedProductQty, VSaleInvoice.TotalQty,  VBaleCount.TotalRugRolls, VBaleCount.TotalFinishedProductRolls, VBaleCount.TotalRolls,  
CASE WHEN VBaleCount.TotalRugRolls <> 0 AND VBaleCount.TotalFinishedProductRolls <> 0        THEN Convert(NVARCHAR,VBaleCount.TotalRugRolls) +  ' + '  + Convert(NVARCHAR,VBaleCount.TotalFinishedProductRolls) + ' = ' + Convert(NVARCHAR,VBaleCount.TotalRolls)       WHEN VBaleCount.TotalRugRolls <> 0 AND VBaleCount.TotalFinishedProductRolls = 0        THEN Convert(NVARCHAR, VBaleCount.TotalRugRolls) 	       WHEN VBaleCount.TotalRugRolls = 0 AND VBaleCount.TotalFinishedProductRolls <> 0        THEN Convert(NVARCHAR,VBaleCount.TotalFinishedProductRolls) 	  END AS TotalRoleText,  
CASE WHEN VSaleInvoice.TotalRugQty <> 0 AND VSaleInvoice.TotalFinishedProductQty <> 0        THEN Convert(NVARCHAR, Convert(INT,VSaleInvoice.TotalRugQty)) +  ' + '  + Convert(NVARCHAR,Convert(INT,VSaleInvoice.TotalFinishedProductQty)) + ' = ' + Convert(NVARCHAR,Convert(INT,VSaleInvoice.TotalQty))        WHEN VSaleInvoice.TotalRugQty <> 0 AND VSaleInvoice.TotalFinishedProductQty = 0         THEN Convert(NVARCHAR,Convert(INT,VSaleInvoice.TotalRugQty))         WHEN VSaleInvoice.TotalRugQty = 0 AND VSaleInvoice.TotalFinishedProductQty <> 0         THEN Convert(NVARCHAR,Convert(INT,VSaleInvoice.TotalFinishedProductQty))      END AS TotalQtyText,   CASE WHEN VSaleInvoice.TotalRugGrossWeight <> 0 AND VSaleInvoice.TotalFinishedProductGrossWeight <> 0         THEN Convert(NVARCHAR, VSaleInvoice.TotalRugGrossWeight) +  ' + '  + Convert(NVARCHAR,VSaleInvoice.TotalFinishedProductGrossWeight) + ' = ' + Convert(NVARCHAR,VSaleInvoice.TotalGrossWeight)        WHEN VSaleInvoice.TotalRugGrossWeight <> 0 AND VSaleInvoice.TotalFinishedProductGrossWeight = 0         THEN Convert(NVARCHAR,VSaleInvoice.TotalRugGrossWeight)         WHEN VSaleInvoice.TotalRugGrossWeight = 0 AND VSaleInvoice.TotalFinishedProductGrossWeight <> 0         THEN Convert(NVARCHAR,VSaleInvoice.TotalFinishedProductGrossWeight)      END AS TotalGrossWeightText,   CASE WHEN VSaleInvoice.TotalRugNetWeight <> 0 AND VSaleInvoice.TotalFinishedProductNetWeight <> 0         THEN Convert(NVARCHAR, VSaleInvoice.TotalRugNetWeight) +  ' + '  + Convert(NVARCHAR,VSaleInvoice.TotalFinishedProductNetWeight) + ' = ' + Convert(NVARCHAR,VSaleInvoice.TotalNetWeight)       WHEN VSaleInvoice.TotalRugNetWeight <> 0 AND VSaleInvoice.TotalFinishedProductNetWeight = 0         THEN Convert(NVARCHAR,VSaleInvoice.TotalRugNetWeight)         WHEN VSaleInvoice.TotalRugNetWeight = 0 AND VSaleInvoice.TotalFinishedProductNetWeight <> 0         THEN Convert(NVARCHAR,VSaleInvoice.TotalFinishedProductNetWeight)      END AS TotalNetWeightText,   
CASE WHEN Prod.UnitId = L.DealUnitId THEN 1 ELSE 0 END AS InvoiceOnProductUnit
FROM  Web.SaleInvoiceHeaders H    
LEFT JOIN Web.SaleInvoiceHeaderDetail Hd ON H.SaleInvoiceHeaderId = Hd.SaleInvoiceHeaderId   
LEFT JOIN Web.SaleDispatchHeaders Dh ON Hd.SaleDispatchHeaderId = Dh.SaleDispatchHeaderId   
LEFT JOIN Web.People P ON H.BillToBuyerId = P.PersonID   
LEFT JOIN (      
	SELECT H.PersonId, Max(H.Address) AS BillToBuyerAddress, Max(C.CityName) AS BillToPartyCity, Max(Con.CountryName) AS BillToPartyCountry       
	FROM  Web.PersonAddresses H        
	LEFT JOIN Web.Cities C ON H.CityId = C.CityId       
	LEFT JOIN Web.States S ON C.StateId = S.StateId       
	LEFT JOIN Web.Countries Con ON S.CountryId = Con.CountryId       
	GROUP BY H.PersonId   
) AS VAddress ON P.PersonId = VAddress.PersonId   
LEFT JOIN Web.Currencies C ON H.CurrencyId = C.ID   
LEFT JOIN Web.DeliveryTerms Dt ON Dh.DeliveryTermsId = Dt.DeliveryTermsId   
LEFT JOIN Web.ShipMethods Sm ON Dh.ShipMethodId = Sm.ShipMethodId   
LEFT JOIN Web.SaleInvoiceLines L  ON H.SaleInvoiceHeaderId = L.SaleInvoiceHeaderId   
LEFT JOIN Web.SaleDispatchLines Dl ON L.SaleDispatchLineId = Dl.SaleDispatchLineId   
LEFT JOIN Web.PackingLines Pl ON Dl.PackingLineId = Pl.PackingLineId    
LEFT JOIN Web.Units Du ON Pl.DealUnitId = Du.UnitId
LEFT JOIN Web.SaleOrderLines Sol ON Pl.SaleOrderLineId = Sol.SaleOrderLineId   
LEFT JOIN Web.SaleOrderHeaders Soh ON Sol.SaleOrderHeaderId = Soh.SaleOrderHeaderId  
LEFT JOIN Web.Products Prod ON Pl.ProductId = Prod.ProductId  
LEFT JOIN Web.FinishedProduct Fp ON Prod.ProductId = Fp.ProductId 
LEFT JOIN Web.ProductGroups Pg ON Prod.ProductGroupId = Pg.ProductGroupId  
LEFT JOIN Web.Colours PCol ON Fp.ColourId = PCol.ColourId   
LEFT JOIN Web.ProductCategories Pc ON Fp.ProductCategoryId = Pc.ProductCategoryId   
LEFT JOIN Web.ProductTypes Pt ON Pc.ProductTypeId = Pt.ProductTypeId    
LEFT JOIN Web.ProductInvoiceGroups Pig ON L.ProductInvoiceGroupId  = Pig.ProductInvoiceGroupId   
LEFT JOIN (       
	SELECT Ps.ProductId, S.SizeName + IsNull(Psh.ProductShapeShortName,'') AS SizeName, S.Area SqFeetArea, IsNull(S.Area,0) * 0.092903 AS SqMeterArea       
	FROM  Web.ProductSizes Ps         
	LEFT JOIN Web.Sizes S ON Ps.SizeId = S.SizeId       
	LEFT JOIN Web.FinishedProduct Fp ON Ps.ProductId = Fp.ProductId
	LEFT JOIN Web.ProductShapes Psh ON Fp.ProductShapeId = Psh.ProductShapeId
	LEFT JOIN Web.ProductSizeTypes Pst ON Ps.ProductSizeTypeId = Pst.ProductSizeTypeId       
	WHERE Pst.ProductSizeTypeName = 'Standard'   
) AS VProductArea ON Prod.ProductId = VProductArea.ProductId   
LEFT JOIN (       
	SELECT L.SaleInvoiceHeaderId,        
	IsNull(Sum(CASE WHEN Pt.ProductTypeName = 'Rug'  THEN Pl.GrossWeight END),0) AS TotalRugGrossWeight,       
	IsNull(Sum(CASE WHEN Pt.ProductTypeName <> 'Rug'  THEN Pl.GrossWeight END),0) AS TotalFinishedProductGrossWeight,       
	IsNull(Sum(Pl.GrossWeight),0) AS TotalGrossWeight,       
	IsNull(Sum(CASE WHEN Pt.ProductTypeName = 'Rug'  THEN Pl.NetWeight END),0) AS TotalRugNetWeight,       
	IsNull(Sum(CASE WHEN Pt.ProductTypeName <> 'Rug'  THEN Pl.NetWeight END),0) AS TotalFinishedProductNetWeight,       
	IsNull(Sum(Pl.NetWeight),0) AS TotalNetWeight,       
	IsNull(Sum(CASE WHEN Pt.ProductTypeName = 'Rug'  THEN Pl.Qty END),0) AS TotalRugQty,       
	IsNull(Sum(CASE WHEN Pt.ProductTypeName <> 'Rug'  THEN Pl.Qty END),0) AS TotalFinishedProductQty,       
	IsNull(Sum(Pl.Qty),0) AS TotalQty,  
	IsNull(Sum(CASE WHEN Pt.ProductTypeName = 'Rug' THEN Pl.DealQty ELSE 0 END),0) AS TotalDealQty, 
	IsNull(Sum(L.Amount),0) AS TotalAmount       
	FROM  Web.SaleInvoiceLines L        
	LEFT JOIN Web.SaleDispatchLines Dl ON L.SaleDispatchLineId = Dl.SaleDispatchLineId       
	LEFT JOIN Web.PackingLines Pl ON PL.PackingLineId = Dl.PackingLineId       
	LEFT JOIN Web.Products P ON PL.ProductId = P.ProductId       
	LEFT JOIN Web.ProductGroups Pg ON P.ProductGroupId = Pg.ProductGroupId       
	LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId       
	WHERE L.SaleInvoiceHeaderId = @Id
	GROUP BY L.SaleInvoiceHeaderId   
) AS VSaleInvoice ON L.SaleInvoiceHeaderId = VSaleInvoice.SaleInvoiceHeaderId   
LEFT JOIN (       
	SELECT H.SaleInvoiceHeaderId,        IsNull(Sum(VBales.RugRolls),0) AS TotalRugRolls,       
	IsNull(Sum(VBales.FinishedRolls),0) AS TotalFinishedProductRolls,       
	IsNull(Sum(VBales.TotalRolls),0) AS TotalRolls       
	FROM  Web.SaleInvoiceHeaders H        
	LEFT JOIN (           
		SELECT DISTINCT  L.SaleInvoiceHeaderId, Pl.BaleNo,           
		CASE WHEN Pt.ProductTypeName = 'Rug' THEN 1 ELSE 0 END  AS RugRolls,           
		CASE WHEN Pt.ProductTypeName <> 'Rug' THEN 
		(CASE WHEN charindex('-', Pl.BaleNo) = 0 THEN 1 ELSE 
		Convert(INT, Reverse(Substring(Reverse(Pl.BaleNo),0,charindex('-',Reverse(Pl.BaleNo))))) -  
		Convert(INT,Substring(Pl.BaleNo,0,charindex('-',Pl.BaleNo))) + 1 
		END) ELSE 0 END  AS FinishedRolls,           
		1 AS TotalRolls           
		FROM  Web.SaleInvoiceLines L            
		LEFT JOIN Web.SaleDispatchLines DL ON L.SaleDispatchLineId = Dl.SaleDispatchLineId           
		LEFT JOIN Web.PackingLines PL ON Dl.PackingLineId = Pl.PackingLineId           
		LEFT JOIN Web.Products P ON Pl.ProductId = P.ProductId           
		LEFT JOIN Web.ProductGroups Pg ON P.ProductGroupId = Pg.ProductGroupId       
		LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId       
		WHERE L.SaleInvoiceHeaderId = @Id
	) AS VBales ON H.SaleInvoiceHeaderId = VBales.SaleInvoiceHeaderId       
	GROUP BY H.SaleInvoiceHeaderId   
) AS VBaleCount ON H.SaleInvoiceHeaderId = VBaleCount.SaleInvoiceHeaderId   
LEFT JOIN (
	SELECT TOP 1 C.SaleInvoiceHeaderId, P.Name AS CourierName
	FROM Web.CustomDetails C 
	LEFT JOIN Web.People P ON C.TRCourierId = P.PersonID
	WHERE C.SaleInvoiceHeaderId = @Id
) AS VCustomDetail ON H.SaleInvoiceHeaderId = VCustomDetail.SaleInvoiceHeaderId
WHERE H.SaleInvoiceHeaderId	= @Id   
End
GO
