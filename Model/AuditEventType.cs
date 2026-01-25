namespace SPOT_API.Models
{
    /// <summary>
    /// Comprehensive audit event types for the system
    /// </summary>
    public enum AuditEventType
    {
        // Authentication Events (1-99)
        Login = 1,
        Logout = 2,
        LoginFailed = 3,
        PasswordChanged = 4,
        PasswordResetRequested = 5,
        PasswordResetCompleted = 6,
        AccountLocked = 7,
        AccountUnlocked = 8,
        TokenRefreshed = 9,

        // Entity CRUD Events (100-199)
        EntityCreated = 100,
        EntityUpdated = 101,
        EntityDeleted = 102,
        EntityRestored = 103,
        EntityViewed = 104,

        // User Management (200-299)
        UserCreated = 200,
        UserUpdated = 201,
        UserDeleted = 202,
        UserActivated = 203,
        UserDeactivated = 204,
        UserRolesAssigned = 205,
        UserRolesRemoved = 206,

        // Role Management (300-399)
        RoleCreated = 300,
        RoleUpdated = 301,
        RoleDeleted = 302,
        RolePermissionsAssigned = 303,
        RolePermissionsRemoved = 304,

        // Stock Management (400-499)
        StockCreated = 400,
        StockUpdated = 401,
        StockDeleted = 402,
        StockPublished = 403,
        StockPurchased = 405,
        StockImported = 406,
        StockSold = 407,
        StockPricingUpdated = 408,
        StockRegistered = 409,

        // Customer Management (500-599)
        CustomerCreated = 500,
        CustomerUpdated = 501,
        CustomerDeleted = 502,
        SaleCompleted = 503,
        CustomerDocumentUploaded = 504,

        // Vehicle Management (550-599)
        VehicleCreated = 550,
        VehicleUpdated = 551,
        VehicleDeleted = 552,

        // Document Management (600-699)
        DocumentUploaded = 600,
        DocumentDeleted = 601,
        DocumentDownloaded = 602,

        // Payment Events (700-799)
        PaymentInitiated = 700,
        PaymentCompleted = 701,
        PaymentFailed = 702,
        PaymentRefunded = 703,
        DisbursementCreated = 704,

        // Report Events (800-899)
        ReportGenerated = 800,
        ReportExported = 801,

        // Master Data Management (900-999)
        BankCreated = 900,
        BankUpdated = 901,
        BankDeleted = 902,
        BrandCreated = 903,
        BrandUpdated = 904,
        BrandDeleted = 905,
        SupplierCreated = 906,
        SupplierUpdated = 907,
        SupplierDeleted = 908,

        // System Events (9000-9999)
        SystemStarted = 9000,
        SystemShutdown = 9001,
        BackupCompleted = 9002,
        DataImportCompleted = 9003,
        MaintenanceModeEnabled = 9004,
        MaintenanceModeDisabled = 9005,
        AuditLogsCleanedUp = 9006,
        ConfigurationChanged = 9007,

        // HTTP/API Events (9100-9199)
        HttpRequestFailed = 9100,
        SystemError = 9101,
        UnauthorizedAccess = 9102,
        UserRegistered = 9104,
        UserRegistrationFailed = 9105,
        EmailVerified = 9106,
        EmailVerificationFailed = 9107,
        RoleAssigned = 9108,
        RoleRevoked = 9109,
        RoleModified = 9110,
        ForbiddenAccess = 9103
    }
}
