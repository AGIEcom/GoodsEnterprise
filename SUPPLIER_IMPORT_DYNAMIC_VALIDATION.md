# Supplier Import - Dynamic Validation from JSON Configuration

## ✅ **Dynamic Validation Implementation Complete!**

The supplier import system now validates required fields and validation rules dynamically based on the JSON configuration file instead of hardcoded data annotations.

## 🎯 **Key Changes Implemented**

### **1. SupplierImport Model Updates**
#### **Before (Hardcoded Data Annotations)**
```csharp
[Required(ErrorMessage = "Supplier Name is required")]
[StringLength(100, ErrorMessage = "Supplier Name cannot exceed 100 characters")]
public string SupplierName { get; set; }

[Required(ErrorMessage = "SKU Code is required")]
[StringLength(50, ErrorMessage = "SKU Code cannot exceed 50 characters")]
public string SKUCode { get; set; }

[Required(ErrorMessage = "Email is required")]
[EmailAddress(ErrorMessage = "Invalid email format")]
[StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
public string Email { get; set; }
```

#### **After (Clean Model with Dynamic Validation)**
```csharp
// All fields are now dynamically validated based on JSON configuration
// Required/Optional status and validation rules are determined at runtime

public string SupplierName { get; set; }
public string SKUCode { get; set; }
public string Email { get; set; }
public string FirstName { get; set; }
// ... other fields without hardcoded validation attributes
```

### **2. SupplierValidationService Updates**

#### **JSON Configuration Loading**
```csharp
private void LoadConfiguration()
{
    var configPath = Path.Combine(_environment.WebRootPath, "config", "supplier-import-columns.json");
    if (File.Exists(configPath))
    {
        var jsonContent = File.ReadAllText(configPath);
        _config = JsonSerializer.Deserialize<SupplierImportConfig>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}
```

#### **Dynamic Validation Logic**
```csharp
private async Task ValidateUsingJsonConfig(SupplierImport supplier, ValidationResult result)
{
    // Validate required fields from JSON config
    var requiredColumns = _config.SupplierImportColumns.RequiredColumns ?? new List<ColumnDefinition>();
    foreach (var column in requiredColumns)
    {
        var value = GetPropertyValue(supplier, column.Name);
        if (string.IsNullOrWhiteSpace(value?.ToString()))
        {
            var error = $"{column.DisplayName ?? column.Name} is required";
            result.Errors.Add(error);
            AddFieldError(result, column.Name, error);
        }
        else
        {
            ValidateFieldRules(supplier, column, result);
        }
    }
}
```

## 🔧 **Dynamic Validation Features**

### **1. Required Field Validation**
- **JSON-Driven**: Required fields determined by `requiredColumns` array in JSON
- **Dynamic Messages**: Error messages use `displayName` from JSON configuration
- **Flexible Configuration**: Easy to add/remove required fields by updating JSON

### **2. Field-Specific Validation Rules**
```csharp
private void ValidateFieldRules(SupplierImport supplier, ColumnDefinition column, ValidationResult result)
{
    // String length validation
    if (column.MaxLength.HasValue && stringValue.Length > column.MaxLength.Value)
    {
        var error = $"{column.DisplayName ?? column.Name} cannot exceed {column.MaxLength} characters";
        result.Errors.Add(error);
    }

    // Type-specific validation
    switch (column.Type?.ToLower())
    {
        case "email":
            if (!IsValidEmail(stringValue)) { /* Add email validation error */ }
            break;
        case "integer":
            if (!int.TryParse(stringValue, out int intValue)) { /* Add integer validation error */ }
            // Range validation for integers
            break;
        case "decimal":
            if (!decimal.TryParse(stringValue, out decimal decimalValue)) { /* Add decimal validation error */ }
            break;
        case "boolean":
            // Validate against acceptedValues for boolean fields
            break;
    }
}
```

### **3. Advanced Validation Rules**

#### **String Length Validation**
```json
{
  "name": "SupplierName",
  "type": "string",
  "maxLength": 100,
  "required": true
}
```

#### **Integer Range Validation**
```json
{
  "name": "LeadTimeDays",
  "type": "integer",
  "min": 0,
  "max": 365,
  "required": false
}
```

#### **Accepted Values Validation**
```json
{
  "name": "Incoterm",
  "type": "string",
  "acceptedValues": ["EXW", "FCA", "CPT", "CIP", "DAT", "DAP", "DDP", "FAS", "FOB", "CFR", "CIF"],
  "required": false
}
```

#### **Email Format Validation**
```json
{
  "name": "Email",
  "type": "email",
  "maxLength": 100,
  "required": true
}
```

## 📊 **JSON Configuration Structure**

### **Updated Configuration with Email as Required**
```json
{
  "supplierImportColumns": {
    "requiredColumns": [
      {
        "name": "SupplierName",
        "displayName": "Supplier Name",
        "type": "string",
        "required": true,
        "maxLength": 100,
        "description": "Name of the supplier (Required)"
      },
      {
        "name": "SKUCode",
        "displayName": "SKU Code", 
        "type": "string",
        "required": true,
        "maxLength": 50,
        "description": "Unique SKU code for the supplier (Required)"
      },
      {
        "name": "Email",
        "displayName": "Email Address",
        "type": "email",
        "required": true,
        "maxLength": 100,
        "description": "Valid email address (Required)"
      }
    ],
    "optionalColumns": [
      // ... optional field definitions
    ],
    "validationRules": {
      "duplicateCheck": {
        "enabled": true,
        "checkFields": ["SKUCode"], // Updated to only check SKUCode
        "action": "reject"
      }
    }
  }
}
```

## 🎯 **Validation Flow**

### **1. Configuration Loading**
```
Service Initialization → Load JSON Config → Parse Column Definitions → Ready for Validation
```

### **2. Validation Process**
```
Supplier Data → JSON Config Validation → Field Rules Validation → Business Rules → Result
```

### **3. Error Handling**
```
Validation Error → Add to Result.Errors → Add to Result.FieldErrors → Continue Validation
```

## 🔧 **Technical Benefits**

### **1. Runtime Flexibility**
- **No Code Changes**: Modify validation rules by updating JSON only
- **Dynamic Configuration**: Add/remove/modify fields without recompilation
- **Environment-Specific**: Different validation rules per environment

### **2. Comprehensive Validation**
- **Type-Safe**: Proper type validation based on column type
- **Range Validation**: Min/max values for numeric fields
- **Format Validation**: Email, date, and other format validations
- **Business Rules**: Accepted values and custom validation logic

### **3. Error Reporting**
- **Detailed Messages**: User-friendly error messages with field display names
- **Field-Level Errors**: Errors grouped by field for better UX
- **Fallback Support**: Graceful degradation if JSON config fails to load

## 📋 **Validation Rules Supported**

### **Field Types**
| Type | Validation | Example |
|------|------------|---------|
| **string** | Length, Required | SupplierName (max 100 chars) |
| **email** | Format, Length, Required | Email validation |
| **integer** | Range, Type, Required | LeadTimeDays (0-365) |
| **decimal** | Range, Type, Required | LastCost (>= 0) |
| **boolean** | Accepted Values | IsActive (true/false/1/0) |
| **date** | Format, Range | ValidFrom/ValidTo dates |

### **Validation Properties**
| Property | Purpose | Example |
|----------|---------|---------|
| **required** | Field is mandatory | `"required": true` |
| **maxLength** | Maximum string length | `"maxLength": 100` |
| **min/max** | Numeric range | `"min": 0, "max": 365` |
| **acceptedValues** | Allowed values | `["EXW", "FCA", "CPT"]` |
| **type** | Data type validation | `"type": "email"` |

## 🚀 **Usage Examples**

### **Adding a New Required Field**
```json
// Add to requiredColumns array in JSON
{
  "name": "CompanyCode",
  "displayName": "Company Code",
  "type": "string",
  "required": true,
  "maxLength": 10,
  "description": "Unique company identifier"
}
```

### **Changing Field from Required to Optional**
```json
// Move from requiredColumns to optionalColumns array
// Change "required": true to "required": false
```

### **Adding Custom Validation Rules**
```json
{
  "name": "Priority",
  "displayName": "Priority Level",
  "type": "string",
  "acceptedValues": ["High", "Medium", "Low"],
  "required": false,
  "description": "Supplier priority level"
}
```

## 🎉 **Result Summary**

### **✅ Completed Features**
1. **Dynamic Required Field Validation**: Based on JSON `requiredColumns`
2. **Type-Specific Validation**: Email, integer, decimal, boolean, string validation
3. **Range and Length Validation**: Min/max values and string length limits
4. **Accepted Values Validation**: Dropdown-style validation for specific fields
5. **Fallback Validation**: Graceful degradation if JSON config unavailable
6. **Comprehensive Error Reporting**: Field-level and general error messages

### **🔧 Technical Improvements**
- **Removed Hardcoded Validation**: Eliminated all data annotation attributes
- **JSON-Driven Configuration**: All validation rules read from JSON file
- **Runtime Flexibility**: Change validation rules without code deployment
- **Enhanced Error Messages**: User-friendly messages with display names
- **Robust Error Handling**: Proper fallback and logging mechanisms

---

**✅ Result: The supplier import system now provides completely dynamic validation based on JSON configuration, offering maximum flexibility and maintainability!**
