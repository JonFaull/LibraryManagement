EXEC sp_addmessage
	@msgnum = 50001,
	@severity = 16,
	@msgtext = N'No available copies of this book',
	@replace = 'replace';