	-- The 'open' order status will have the sort order of 2 
	UPDATE merchOrderStatus 
	SET sortOrder = sortOrder + 1
	WHERE sortOrder > 1

	-- Add the 'open' order status
	INSERT INTO merchOrderStatus
	(pk, name, alias, reportable, active, sortOrder, updateDate, createDate)
	VALUES
	('e67b414e-0e55-480d-9429-1204ad46ecda', 'Open', 'open', 1, 1, 2, GETDATE(), GETDATE())