-- the product
SELECT	* 
FROM	[dbo].[merchProduct]			T1
JOIN	[dbo].[merchProductVariant]		T2
ON		T1.pk = T2.productKey
WHERE	T2.template = 1

-- product options
SELECT	*
FROM	merchProductOption				T1
JOIN	merchProduct2ProductOption		T2
ON		T1.id = T2.optionId


-- product option choices (attributes)
SELECT	*
FROM	merchProductAttribute			T1
JOIN	merchProductOption				T2
ON		T1.optionId = T2.id