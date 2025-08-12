EXEC sp_addmessage
	@msgnum = 50001,
	@severity = 16,
	@msgtext = N'No available copies of this book',
	@replace = 'replace';

EXEC sp_addmessage
    @msgnum = 50002,
    @severity = 16,
    @msgtext = N'This student already has this book checked out.',
    @replace = 'replace';