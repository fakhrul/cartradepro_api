# Production Deployment Guide

## Overview

This guide covers the deployment of critical fixes to the CarTradePro production server. These fixes address:

1. **Database Connectivity** - PostgreSQL connection from Docker containers
2. **User Creation** - Automatic UserRole record creation for new users
3. **Registration Role Permissions** - Proper permission initialization

**Date Created:** 2026-01-28
**Tested On:** Development server
**Target:** Production server (178.128.105.21)

---

## Prerequisites

### Required Access
- SSH access to production server (root@ubuntu-s-4vcpu-8gb-intel-sgp1-01)
- Access to Harbor registry: `harbor.safa.com.my`
- Database access credentials
- API running in Docker container

### Required Information
- **Production Database:** 172.17.0.1:5433 (PostgreSQL 10)
- **Database Name:** cartradepro
- **Database User:** postgres
- **Database Password:** Str0ng_P@ssw0rd_2026
- **API Container Name:** cartradeproapi
- **API Port:** 8101

---

## Part 1: Server Configuration (One-Time Setup)

### Step 1: Configure UFW Firewall

SSH into the production server and run:

```bash
# Check current UFW status
sudo ufw status numbered

# Remove any DENY rules for port 5432 (if they exist)
# Replace X with the rule number from 'ufw status numbered'
sudo ufw delete X

# Allow PostgreSQL connections from Docker network
sudo ufw allow from 172.17.0.0/16 to any port 5432

# Verify the rule was added
sudo ufw status numbered
```

**Expected Result:**
```
To                         Action      From
--                         ------      ----
5432                       ALLOW IN    172.17.0.0/16
```

### Step 2: Configure PostgreSQL

```bash
# Verify PostgreSQL is listening on Docker bridge IP
sudo nano /etc/postgresql/10/main/postgresql.conf
```

Ensure this line exists (uncommented):
```
listen_addresses = 'localhost,172.17.0.1'
```

**Configure pg_hba.conf:**

```bash
# Edit pg_hba.conf
sudo nano /etc/postgresql/10/main/pg_hba.conf
```

Add this line near the top (before local entries):
```
host    all    all    172.17.0.0/16    md5
```

**Reload PostgreSQL:**
```bash
sudo systemctl reload postgresql

# Verify PostgreSQL is listening
sudo netstat -plnt | grep postgres
```

**Expected Output:**
```
tcp  0  0  127.0.0.1:5432     0.0.0.0:*  LISTEN  <pid>/postgres
tcp  0  0  172.17.0.1:5432    0.0.0.0:*  LISTEN  <pid>/postgres
```

---

## Part 2: Build and Deploy Updated API

### Step 1: Build Docker Image (On Local Machine)

```bash
# Navigate to API directory
cd C:\GitHub\cartradepro_api

# Verify the latest changes are pulled
git pull origin main

# Verify commits are present
git log --oneline -5
```

**Expected commits:**
- `24fa958` - Add endpoint to fix Registration role permissions
- `94d6757` - Fix 'User has no active roles' login error for newly created users
- `045af6c` - Fix Production database connection string for Docker deployment

**Build the Docker image:**

```bash
# Build with production tag
docker build -t harbor.safa.com.my/cartradepro/cartradeproapi:latest \
             -t harbor.safa.com.my/cartradepro/cartradeproapi:v2026.01.28 \
             -f SPOT_API/Dockerfile .

# Verify build succeeded
docker images | grep cartradeproapi
```

### Step 2: Push to Harbor Registry

```bash
# Login to Harbor (if not already logged in)
docker login harbor.safa.com.my

# Push latest tag
docker push harbor.safa.com.my/cartradepro/cartradeproapi:latest

# Push versioned tag (for rollback capability)
docker push harbor.safa.com.my/cartradepro/cartradeproapi:v2026.01.28
```

### Step 3: Deploy to Production Server

SSH into production server:

```bash
ssh root@178.128.105.21
```

**Pull and deploy:**

```bash
# Pull latest image
docker pull harbor.safa.com.my/cartradepro/cartradeproapi:latest

# Stop the current container
docker stop cartradeproapi

# Remove the old container
docker rm cartradeproapi

# Run the new container
# NOTE: Adjust the docker run command based on your existing configuration
docker run -d \
  --name cartradeproapi \
  -p 8101:80 \
  --restart unless-stopped \
  harbor.safa.com.my/cartradepro/cartradeproapi:latest

# Verify container is running
docker ps | grep cartradeproapi

# Check container logs for any errors
docker logs -f cartradeproapi
```

**Expected logs:**
- No database connection errors
- Application started successfully
- Migrations applied (if any)

**Press Ctrl+C to exit log view**

---

## Part 3: Verify Database Connectivity

### Test the Health API Endpoint

```bash
# From production server or your local machine
curl https://api.sinargroup.my/api/Health/database
```

**Expected Response:**
```json
{
  "version": "2.0",
  "message": "GET Request successful.",
  "result": {
    "status": "Healthy",
    "message": "Database connection successful",
    "database": {
      "name": "cartradepro",
      "provider": "Npgsql.EntityFrameworkCore.PostgreSQL",
      "canConnect": true,
      "statistics": {
        "userCount": <number>,
        "stockCount": <number>
      },
      "migrations": {
        "hasPendingMigrations": false
      }
    }
  }
}
```

**If `canConnect: false`:**
1. Check UFW rules: `sudo ufw status | grep 5432`
2. Check PostgreSQL is listening: `sudo netstat -plnt | grep 5432`
3. Check pg_hba.conf: `sudo cat /etc/postgresql/10/main/pg_hba.conf | grep 172.17`
4. Check container logs: `docker logs cartradeproapi`

---

## Part 4: Fix Registration Role Permissions

### Step 1: Run the Fix Endpoint

```bash
# Call the fix endpoint
curl https://api.sinargroup.my/api/Roles/fix-registration-permissions
```

**Expected Response:**
```json
{
  "version": "2.0",
  "message": "GET Request successful.",
  "result": {
    "success": true,
    "message": "Registration role permissions fixed: 8 updated, 0 created",
    "updated": 8,
    "created": 0,
    "details": [
      "Found Registration role with ID: <guid>",
      "Updated permission for 'Stock Info' (Code: STOCK_INFO)",
      "Updated permission for 'Vehicle Info' (Code: VEHICLE_INFO)",
      "Updated permission for 'Clearance' (Code: CLEARANCE)",
      "Updated permission for 'Sale' (Code: SALE)",
      "Updated permission for 'Registration' (Code: REGISTRATION)",
      "Updated permission for 'Expenses' (Code: EXPENSES)",
      "Updated permission for 'Administrative Cost' (Code: ADMINISTRATIVE_COST)",
      "Updated permission for 'Disbursement' (Code: DISBURSEMENT)",
      "Updated STOCKS module permission to CanView=true"
    ]
  }
}
```

**If the response shows errors:**
- Check that the Registration role exists in the database
- Check that the SubModules exist (VEHICLE_INFO, STOCK_INFO, etc.)
- Review the error message in the response

### Step 2: Verify Permissions in Database (Optional)

```bash
# Connect to PostgreSQL
psql -h 172.17.0.1 -p 5433 -U postgres -d cartradepro

# Check Registration role permissions
SELECT
  r."Name" as Role,
  sm."Name" as SubModule,
  sm."Code" as SubModuleCode,
  rsmp."CanView",
  rsmp."CanAdd",
  rsmp."CanUpdate",
  rsmp."CanDelete"
FROM "RoleSubModulePermissions" rsmp
JOIN "Roles" r ON rsmp."RoleId" = r."Id"
JOIN "SubModules" sm ON rsmp."SubModuleId" = sm."Id"
WHERE r."Name" = 'Registration'
AND sm."Code" IN ('STOCK_INFO', 'VEHICLE_INFO', 'CLEARANCE', 'SALE',
                  'REGISTRATION', 'EXPENSES', 'ADMINISTRATIVE_COST', 'DISBURSEMENT')
ORDER BY sm."Name";

# Exit psql
\q
```

**Expected Output:** All permissions should show `true` for CanView, CanAdd, CanUpdate, CanDelete

---

## Part 5: Test the Fixes

### Test 1: Create a New User

1. **Access admin panel:** https://sinargroup.my/admin
2. **Navigate to User Management**
3. **Create a new test user:**
   - Name: Test User
   - Email: testuser@example.com
   - Password: TestPassword123
   - Role: Registration
4. **Save the user**

### Test 2: Login with New User

1. **Logout from admin account**
2. **Clear browser localStorage:**
   - Open DevTools (F12)
   - Console tab
   - Run: `localStorage.clear()`
3. **Login with test user credentials:**
   - Email: testuser@example.com
   - Password: TestPassword123
4. **Expected Result:** Login succeeds (no "User has no active roles" error)

### Test 3: Verify Vehicle Info Tab

1. **Navigate to Stock List:** https://sinargroup.my/admins/StockList
2. **Click on any stock** to view details
3. **Expected Result:** "Vehicle Info" tab is visible and accessible

### Test 4: Verify Other Tabs

Check that all these tabs are visible in Stock detail page:
- ✓ Stock Info (may be hidden - check permissions)
- ✓ Vehicle Info
- ✓ Purchase
- ✓ Import
- ✓ Clearance
- ✓ Sale
- ✓ Registration
- ✓ Expenses
- ✓ Administrative Cost
- ✓ Disbursement

---

## Part 6: Monitoring and Verification

### Check Application Logs

```bash
# View recent logs
docker logs --tail 100 cartradeproapi

# Follow logs in real-time
docker logs -f cartradeproapi
```

**Look for:**
- No database connection errors
- Successful login attempts
- No permission-related errors

### Check Browser Console

When users login, check browser console (F12) for:

```
=== User logged in ===
roleSubModulePermissions: (23) [...]
```

Expand the array and verify VEHICLE_INFO and STOCK_INFO are present with `CanView: true`

---

## Troubleshooting

### Issue: Database Connection Failed

**Symptoms:** Health API shows `canConnect: false`

**Solutions:**
1. Verify UFW allows Docker network:
   ```bash
   sudo ufw status | grep 172.17
   ```
2. Verify PostgreSQL pg_hba.conf:
   ```bash
   sudo cat /etc/postgresql/10/main/pg_hba.conf | grep 172.17
   ```
3. Restart PostgreSQL:
   ```bash
   sudo systemctl restart postgresql
   ```
4. Restart API container:
   ```bash
   docker restart cartradeproapi
   ```

### Issue: Vehicle Info Tab Not Showing

**Symptoms:** Tab doesn't appear for Registration role users

**Solutions:**
1. Run the fix endpoint again:
   ```bash
   curl https://api.sinargroup.my/api/Roles/fix-registration-permissions
   ```
2. Verify permissions in database (see Step 2 in Part 4)
3. Have user logout completely and login again
4. Clear browser localStorage: `localStorage.clear()`

### Issue: "User has no active roles"

**Symptoms:** New users can't login

**Solutions:**
1. Verify the user was created through the UI (not directly in database)
2. Check UserRoles table:
   ```sql
   SELECT ur.*, r."Name", u."UserName"
   FROM "UserRoles" ur
   JOIN "Roles" r ON ur."RoleId" = r."Id"
   JOIN "AspNetUsers" u ON ur."UserId" = u."Id"
   WHERE u."UserName" = 'testuser@example.com';
   ```
3. If no UserRole exists, the fix didn't apply. Rebuild and redeploy the API.

### Issue: Docker Container Won't Start

**Symptoms:** `docker ps` doesn't show cartradeproapi

**Solutions:**
1. Check container logs:
   ```bash
   docker logs cartradeproapi
   ```
2. Verify port 8101 isn't in use:
   ```bash
   sudo netstat -plnt | grep 8101
   ```
3. Check appsettings.Production.json in container:
   ```bash
   docker exec cartradeproapi cat /app/appsettings.Production.json
   ```

---

## Rollback Procedure

If issues occur after deployment:

### Rollback to Previous Version

```bash
# Stop current container
docker stop cartradeproapi
docker rm cartradeproapi

# Pull previous version (adjust tag as needed)
docker pull harbor.safa.com.my/cartradepro/cartradeproapi:v2026.01.27

# Run previous version
docker run -d \
  --name cartradeproapi \
  -p 8101:80 \
  --restart unless-stopped \
  harbor.safa.com.my/cartradepro/cartradeproapi:v2026.01.27

# Verify it's running
docker ps | grep cartradeproapi
```

### Revert Database Changes

The fix endpoint only updates existing permissions (doesn't delete anything), so no database rollback is needed. However, if you want to undo the permission changes:

```sql
-- Connect to database
psql -h 172.17.0.1 -p 5433 -U postgres -d cartradepro

-- Revert Registration role permissions to false (if needed)
UPDATE "RoleSubModulePermissions" rsmp
SET "CanView" = false,
    "CanAdd" = false,
    "CanUpdate" = false,
    "CanDelete" = false
FROM "Roles" r, "SubModules" sm
WHERE rsmp."RoleId" = r."Id"
AND rsmp."SubModuleId" = sm."Id"
AND r."Name" = 'Registration'
AND sm."Code" IN ('STOCK_INFO', 'VEHICLE_INFO', 'CLEARANCE', 'SALE',
                  'REGISTRATION', 'EXPENSES', 'ADMINISTRATIVE_COST', 'DISBURSEMENT');
```

---

## Post-Deployment Checklist

- [ ] UFW firewall configured to allow Docker network access to PostgreSQL
- [ ] pg_hba.conf updated to allow Docker network connections
- [ ] PostgreSQL reloaded successfully
- [ ] Docker image built and pushed to Harbor
- [ ] API container deployed and running
- [ ] Health API shows database connection successful
- [ ] Registration role permissions fixed via API endpoint
- [ ] New user creation tested successfully
- [ ] Login with new user tested successfully
- [ ] Vehicle Info tab visible for Registration role
- [ ] No errors in application logs
- [ ] No errors in browser console

---

## Summary of Changes

### Files Modified

1. **C:\GitHub\cartradepro_api\SPOT_API\appsettings.Production.json**
   - Changed connection string to use `172.17.0.1:5433` (Docker bridge IP)
   - Fixed database credentials

2. **C:\GitHub\cartradepro_api\SPOT_API\Controllers\ProfilesController.cs**
   - Added UserRole record creation when new users are created
   - Ensures users can login immediately after creation

3. **C:\GitHub\cartradepro_api\SPOT_API\Controllers\RolesController.cs**
   - Added `GET /api/Roles/fix-registration-permissions` endpoint
   - Fixes Registration role permissions to match expected configuration

### Git Commits

- `24fa958` - Add endpoint to fix Registration role permissions
- `94d6757` - Fix 'User has no active roles' login error for newly created users
- `045af6c` - Fix Production database connection string for Docker deployment

---

## Support Information

**Deployment Date:** _____________
**Deployed By:** _____________
**Verification By:** _____________

**Contact for Issues:**
- Development Team: _____________
- Database Admin: _____________
- Server Admin: _____________

---

## Notes

- This deployment includes breaking changes to the database connection configuration
- UFW and pg_hba.conf must be configured correctly before deployment
- The fix endpoint should be run once after deployment
- All existing users are unaffected (only impacts new user creation and Registration role)
- Backend changes are backward compatible with existing frontend

**End of Deployment Guide**
