$filePath = "C:\GitHub\cartradepro_api\SPOT_API\Services\StockImportService.cs"
$content = Get-Content $filePath -Raw

# Replace Brand section
$content = $content -replace '(?s)// Set brand if provided\s+if \(!string\.IsNullOrWhiteSpace\(row\.BrandName\) && brands\.ContainsKey\(row\.BrandName\.ToLower\(\)\)\)\s+\{\s+vehicle\.BrandId = brands\[row\.BrandName\.ToLower\(\)\];\s+\}', @'
// Set brand if provided - auto-create if doesn't exist (case-insensitive)
                                if (!string.IsNullOrWhiteSpace(row.BrandName))
                                {
                                    var brandKey = row.BrandName.ToLower();
                                    if (!brands.ContainsKey(brandKey))
                                    {
                                        var newBrand = new Brand { Name = row.BrandName.Trim() };
                                        await _context.Brands.AddAsync(newBrand);
                                        await _context.SaveChangesAsync();
                                        brands[brandKey] = newBrand.Id;
                                    }
                                    vehicle.BrandId = brands[brandKey];
                                }
'@

# Replace Model section
$content = $content -replace '(?s)// Set model if provided\s+if \(!string\.IsNullOrWhiteSpace\(row\.ModelName\) && models\.ContainsKey\(row\.ModelName\.ToLower\(\)\)\)\s+\{\s+vehicle\.ModelId = models\[row\.ModelName\.ToLower\(\)\];\s+\}', @'
// Set model if provided - auto-create if doesn't exist (case-insensitive)
                                if (!string.IsNullOrWhiteSpace(row.ModelName))
                                {
                                    var modelKey = row.ModelName.ToLower();
                                    if (!models.ContainsKey(modelKey))
                                    {
                                        var newModel = new Model { Name = row.ModelName.Trim() };
                                        await _context.Models.AddAsync(newModel);
                                        await _context.SaveChangesAsync();
                                        models[modelKey] = newModel.Id;
                                    }
                                    vehicle.ModelId = models[modelKey];
                                }
'@

# Replace VehicleType section
$content = $content -replace '(?s)// Set vehicle type if provided\s+if \(!string\.IsNullOrWhiteSpace\(row\.VehicleTypeName\) && vehicleTypes\.ContainsKey\(row\.VehicleTypeName\.ToLower\(\)\)\)\s+\{\s+vehicle\.VehicleTypeId = vehicleTypes\[row\.VehicleTypeName\.ToLower\(\)\];\s+\}', @'
// Set vehicle type if provided - auto-create if doesn't exist (case-insensitive)
                                if (!string.IsNullOrWhiteSpace(row.VehicleTypeName))
                                {
                                    var vehicleTypeKey = row.VehicleTypeName.ToLower();
                                    if (!vehicleTypes.ContainsKey(vehicleTypeKey))
                                    {
                                        var newVehicleType = new VehicleType { Name = row.VehicleTypeName.Trim() };
                                        await _context.VehicleTypes.AddAsync(newVehicleType);
                                        await _context.SaveChangesAsync();
                                        vehicleTypes[vehicleTypeKey] = newVehicleType.Id;
                                    }
                                    vehicle.VehicleTypeId = vehicleTypes[vehicleTypeKey];
                                }
'@

# Replace Supplier section
$content = $content -replace '(?s)// Set supplier if provided\s+if \(!string\.IsNullOrWhiteSpace\(row\.SupplierName\) && suppliers\.ContainsKey\(row\.SupplierName\.ToLower\(\)\)\)\s+\{\s+purchase\.SupplierId = suppliers\[row\.SupplierName\.ToLower\(\)\];\s+\}', @'
// Set supplier if provided - auto-create if doesn't exist (case-insensitive)
                                if (!string.IsNullOrWhiteSpace(row.SupplierName))
                                {
                                    var supplierKey = row.SupplierName.ToLower();
                                    if (!suppliers.ContainsKey(supplierKey))
                                    {
                                        var newSupplier = new Supplier { Name = row.SupplierName.Trim() };
                                        await _context.Suppliers.AddAsync(newSupplier);
                                        await _context.SaveChangesAsync();
                                        suppliers[supplierKey] = newSupplier.Id;
                                    }
                                    purchase.SupplierId = suppliers[supplierKey];
                                }
'@

# Replace Showroom section
$content = $content -replace '(?s)// Set showroom if provided\s+if \(!string\.IsNullOrWhiteSpace\(row\.ShowRoomLotNo\) && showrooms\.ContainsKey\(row\.ShowRoomLotNo\.ToLower\(\)\)\)\s+\{\s+var showroomId = showrooms\[row\.ShowRoomLotNo\.ToLower\(\)\];\s+stock\.ShowRoomId = showroomId;\s+\}', @'
// Set showroom if provided - auto-create if doesn't exist (case-insensitive)
                                if (!string.IsNullOrWhiteSpace(row.ShowRoomLotNo))
                                {
                                    var showroomKey = row.ShowRoomLotNo.ToLower();
                                    if (!showrooms.ContainsKey(showroomKey))
                                    {
                                        var newShowroom = new ShowRoom { LotNo = row.ShowRoomLotNo.Trim(), Name = row.ShowRoomLotNo.Trim() };
                                        await _context.ShowRooms.AddAsync(newShowroom);
                                        await _context.SaveChangesAsync();
                                        showrooms[showroomKey] = newShowroom.Id;
                                    }
                                    stock.ShowRoomId = showrooms[showroomKey];
                                }
'@

Set-Content $filePath $content -NoNewline
Write-Host "Updated StockImportService.cs with auto-create logic"
