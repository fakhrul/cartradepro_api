import re

# Read the file
with open('SPOT_API/Services/StockImportService.cs', 'r', encoding='utf-8') as f:
    content = f.read()

# Pattern 1: Brand ID assignment (lines 351-355)
old_brand = r'''                                // Set brand if provided
                                if \(!string\.IsNullOrWhiteSpace\(row\.BrandName\) && brands\.ContainsKey\(row\.BrandName\.ToLower\(\)\)\)
                                \{
                                    vehicle\.BrandId = brands\[row\.BrandName\.ToLower\(\)\];
                                \}'''

new_brand = '''                                // Set brand if provided (auto-create if doesn't exist)
                                if (!string.IsNullOrWhiteSpace(row.BrandName))
                                {
                                    var brandKey = row.BrandName.Trim().ToLower();
                                    if (!brands.ContainsKey(brandKey))
                                    {
                                        var newBrand = new Brand { Name = row.BrandName.Trim() };
                                        await _context.Brands.AddAsync(newBrand);
                                        await _context.SaveChangesAsync();
                                        brands[brandKey] = newBrand.Id;
                                    }
                                    vehicle.BrandId = brands[brandKey];
                                }'''

content = re.sub(old_brand, new_brand, content)

# Pattern 2: Model ID assignment (lines 357-361)
old_model = r'''                                // Set model if provided
                                if \(!string\.IsNullOrWhiteSpace\(row\.ModelName\) && models\.ContainsKey\(row\.ModelName\.ToLower\(\)\)\)
                                \{
                                    vehicle\.ModelId = models\[row\.ModelName\.ToLower\(\)\];
                                \}'''

new_model = '''                                // Set model if provided (auto-create if doesn't exist)
                                if (!string.IsNullOrWhiteSpace(row.ModelName))
                                {
                                    var modelKey = row.ModelName.Trim().ToLower();
                                    if (!models.ContainsKey(modelKey))
                                    {
                                        var newModel = new Model { Name = row.ModelName.Trim() };
                                        await _context.Models.AddAsync(newModel);
                                        await _context.SaveChangesAsync();
                                        models[modelKey] = newModel.Id;
                                    }
                                    vehicle.ModelId = models[modelKey];
                                }'''

content = re.sub(old_model, new_model, content)

# Pattern 3: VehicleType ID assignment (lines 363-367)
old_vehicletype = r'''                                // Set vehicle type if provided
                                if \(!string\.IsNullOrWhiteSpace\(row\.VehicleTypeName\) && vehicleTypes\.ContainsKey\(row\.VehicleTypeName\.ToLower\(\)\)\)
                                \{
                                    vehicle\.VehicleTypeId = vehicleTypes\[row\.VehicleTypeName\.ToLower\(\)\];
                                \}'''

new_vehicletype = '''                                // Set vehicle type if provided (auto-create if doesn't exist)
                                if (!string.IsNullOrWhiteSpace(row.VehicleTypeName))
                                {
                                    var vehicleTypeKey = row.VehicleTypeName.Trim().ToLower();
                                    if (!vehicleTypes.ContainsKey(vehicleTypeKey))
                                    {
                                        var newVehicleType = new VehicleType { Name = row.VehicleTypeName.Trim() };
                                        await _context.VehicleTypes.AddAsync(newVehicleType);
                                        await _context.SaveChangesAsync();
                                        vehicleTypes[vehicleTypeKey] = newVehicleType.Id;
                                    }
                                    vehicle.VehicleTypeId = vehicleTypes[vehicleTypeKey];
                                }'''

content = re.sub(old_vehicletype, new_vehicletype, content)

# Pattern 4: Supplier ID assignment (lines 371-375)
old_supplier = r'''                                // Set supplier if provided
                                if \(!string\.IsNullOrWhiteSpace\(row\.SupplierName\) && suppliers\.ContainsKey\(row\.SupplierName\.ToLower\(\)\)\)
                                \{
                                    purchase\.SupplierId = suppliers\[row\.SupplierName\.ToLower\(\)\];
                                \}'''

new_supplier = '''                                // Set supplier if provided (auto-create if doesn't exist)
                                if (!string.IsNullOrWhiteSpace(row.SupplierName))
                                {
                                    var supplierKey = row.SupplierName.Trim().ToLower();
                                    if (!suppliers.ContainsKey(supplierKey))
                                    {
                                        var newSupplier = new Supplier { Name = row.SupplierName.Trim() };
                                        await _context.Suppliers.AddAsync(newSupplier);
                                        await _context.SaveChangesAsync();
                                        suppliers[supplierKey] = newSupplier.Id;
                                    }
                                    purchase.SupplierId = suppliers[supplierKey];
                                }'''

content = re.sub(old_supplier, new_supplier, content)

# Pattern 5: ShowRoom ID assignment (lines 467-472)
old_showroom = r'''                                // Set showroom if provided
                                if \(!string\.IsNullOrWhiteSpace\(row\.ShowRoomLotNo\) && showrooms\.ContainsKey\(row\.ShowRoomLotNo\.ToLower\(\)\)\)
                                \{
                                    var showroomId = showrooms\[row\.ShowRoomLotNo\.ToLower\(\)\];
                                    stock\.ShowRoomId = showroomId;
                                \}'''

new_showroom = '''                                // Set showroom if provided (auto-create if doesn't exist)
                                if (!string.IsNullOrWhiteSpace(row.ShowRoomLotNo))
                                {
                                    var showroomKey = row.ShowRoomLotNo.Trim().ToLower();
                                    if (!showrooms.ContainsKey(showroomKey))
                                    {
                                        var newShowRoom = new ShowRoom
                                        {
                                            LotNo = row.ShowRoomLotNo.Trim(),
                                            Name = row.ShowRoomLotNo.Trim()
                                        };
                                        await _context.ShowRooms.AddAsync(newShowRoom);
                                        await _context.SaveChangesAsync();
                                        showrooms[showroomKey] = newShowRoom.Id;
                                    }
                                    stock.ShowRoomId = showrooms[showroomKey];
                                }'''

content = re.sub(old_showroom, new_showroom, content)

# Write the updated content
with open('SPOT_API/Services/StockImportService.cs', 'w', encoding='utf-8') as f:
    f.write(content)

print("File updated successfully!")
