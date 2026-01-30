# SPOT_API

## Development Environment Preparation

### To drop all postgress table

Method 1
````
DROP SCHEMA public CASCADE;
CREATE SCHEMA public;
````

Method 2
````
DO $$ DECLARE
    r RECORD;
BEGIN
    FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = current_schema()) LOOP
        EXECUTE 'DROP TABLE IF EXISTS ' || quote_ident(r.tablename) || ' CASCADE';
    END LOOP;
END $$;
````

## Adding UUID as primary key in postgres

Reference: [https://arctype.com/blog/postgres-uuid/](https://arctype.com/blog/postgres-uuid/)
````
SELECT * FROM pg_extension

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE PINK_FLOYD (
	id uuid DEFAULT uuid_generate_v4 (),
	album_name VARCHAR NOT NULL,
	release_date DATE NOT NULL,
	PRIMARY KEY (id)
);

````
## IOT Database Script

````
CREATE SEQUENCE devicedata_id_seq;

CREATE TABLE public.devicedata
(
    deviceid character varying COLLATE pg_catalog."default" NOT NULL,
    createdate timestamp without time zone NOT NULL DEFAULT now(),
    data character varying COLLATE pg_catalog."default",
    latitude character varying COLLATE pg_catalog."default",
    longitude character varying COLLATE pg_catalog."default",
    alarm character varying COLLATE pg_catalog."default",
    batt character varying COLLATE pg_catalog."default",
    mode character varying COLLATE pg_catalog."default",
    fw_ver character varying COLLATE pg_catalog."default",
    hw_ver character varying COLLATE pg_catalog."default",
    wifi_rssi character varying COLLATE pg_catalog."default",
    ble_rssi character varying COLLATE pg_catalog."default",
    date_time character varying COLLATE pg_catalog."default",
    prox_id_list character varying COLLATE pg_catalog."default",
    id integer NOT NULL DEFAULT nextval('devicedata_id_seq'::regclass),
    dashboard_status character varying COLLATE pg_catalog."default"
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

````
## Role-Based Access Control System

### Overview

The application uses a JSONB-based permission system stored in the `Roles` table. This system provides hierarchical module and submodule permissions that control access to different parts of the application.

### Architecture

#### 1. Backend - Module Provider (Static)

The module and submodule structure is hardcoded in `Application/Services/ModulesProvider.cs`. This replaces the old database-stored modules/submodules approach.

Key modules defined:
- Dashboard (`DASHBOARD`)
- Stock To Buy (`STOCK_TO_BUY`)
- Stocks (`STOCKS`) - with 13 submodules (Sale, Expenses, etc.)
- Customer (`CUSTOMER`)
- Pricelist (`VEHICLE`)
- Advertisement (`ADVERTISEMENT`)
- MasterData (`MASTER_DATA`) - with 5 submodules
- Reports (`REPORT`)
- Administration (`ADMINISTRATION`) - with 4 submodules

#### 2. Backend - JSONB Permissions

Role permissions are stored in the `Roles.Permissions` column as JSONB with the following structure:

```json
{
  "ModuleName": {
    "CanView": true,
    "CanAdd": false,
    "CanUpdate": false,
    "CanDelete": false,
    "SubModules": {
      "SubModuleName": {
        "CanView": true,
        "CanAdd": false,
        "CanUpdate": false,
        "CanDelete": false
      }
    }
  }
}
```

Example for a Cashier role:
```json
{
  "Stock To Buy": {
    "CanView": true,
    "CanAdd": true,
    "CanUpdate": true,
    "CanDelete": false
  },
  "Stocks": {
    "CanView": true,
    "SubModules": {
      "Sale": {
        "CanView": true,
        "CanAdd": true,
        "CanUpdate": true,
        "CanDelete": false
      },
      "Expenses": {
        "CanView": true,
        "CanAdd": true,
        "CanUpdate": true,
        "CanDelete": false
      }
    }
  }
}
```

#### 3. Frontend - Menu Filtering

The frontend navigation menu (`src/store/menu.js`) uses `moduleCode` and `subModuleCode` properties to match against user permissions.

Menu filtering happens in `src/utils/filterMenu.js`:
- Compares menu item codes with user's role permissions
- Filters out menu items the user doesn't have `CanView` permission for
- Supports nested submodule permissions

Example menu structure:
```javascript
{
  _name: 'CSidebarNavDropdown',
  name: 'Stock',
  moduleCode: 'STOCKS',
  _children: [
    {
      _name: 'CSidebarNavItem',
      name: 'Sale',
      subModuleCode: 'SALE'
    }
  ]
}
```

### Important: Module Code Consistency

The `Code` property in backend `ModulesProvider.cs` MUST match the `moduleCode` in frontend `menu.js`:

| Module | Backend Code | Frontend moduleCode |
|--------|-------------|---------------------|
| Dashboard | `DASHBOARD` | `DASHBOARD` |
| Stock To Buy | `STOCK_TO_BUY` | `STOCK_TO_BUY` |
| Stocks | `STOCKS` | `STOCKS` |
| Customer | `CUSTOMER` | `CUSTOMER` |
| Pricelist | `VEHICLE` | `VEHICLE` |
| Advertisement | `ADVERTISEMENT` | `ADVERTISEMENT` |
| MasterData | `MASTER_DATA` | `MASTER_DATA` |
| Reports | `REPORT` | `REPORT` |
| Administration | `ADMINISTRATION` | `ADMINISTRATION` |

### Creating/Updating Roles

1. **Via API Endpoint**: `POST /api/Roles` or `PUT /api/Roles/{id}`
   - Frontend: Uses `Role.vue` form
   - Backend: `RolesController` converts the request to JSONB permissions

2. **Getting Modules Template**: `GET /api/Roles/modules-template`
   - Returns the complete hardcoded module structure
   - Used by frontend to build the role creation form

### Permission Checking

#### Backend
```csharp
// In AuthorizationService.cs
var permissions = ModulesProvider.GetModules();
var hasPermission = role.Permissions["ModuleName"]["CanView"];
```

#### Frontend
```javascript
// In filterMenu.js
const allowedModuleCodes = roleModulePermissions
  .filter(p => p.canView)
  .map(p => p.module.code);
```

### Key Files

**Backend:**
- `Application/Services/ModulesProvider.cs` - Module structure definition
- `Application/Services/AuthorizationService.cs` - Permission checking
- `SPOT_API/Controllers/RolesController.cs` - Role CRUD operations
- `SPOT_API/DTOs/RoleDto.cs` - Request/response DTOs

**Frontend:**
- `src/store/menu.js` - Navigation menu structure
- `src/utils/filterMenu.js` - Menu filtering logic
- `src/views/admins/Role.vue` - Role create/edit form
- `src/views/admins/RoleList.vue` - Role management list

### Migration Notes

The system has migrated from database-stored Modules/SubModules tables to:
1. Hardcoded module structure in `ModulesProvider.cs`
2. JSONB permissions in `Roles.Permissions` column
3. Menu filtering based on module codes

This provides better performance and easier maintenance compared to the old relational approach.

## Deployment Using Visual Studio

1. Please ensure that only the web api is in the project. Other project such as console (for web jobs) need to unload first)
2. Right click on the SPOT API project and select publish
3. Click publish
