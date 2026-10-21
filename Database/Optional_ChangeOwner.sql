/*
    DO NOT run as part of normal setup.

    The supplied source ends with this command, but it requires an appropriate
    SQL Server principal and is not needed by MVC5/EF6. Run it only when the
    lecturer explicitly requires it and you have confirmed the target instance.
*/
ALTER AUTHORIZATION ON DATABASE::[MyStore] TO [sa];
GO
