# Import Functionality Troubleshooting Guide

## Current Status
- ✅ ProductImportController created
- ✅ PromotionCostImportController created  
- ✅ BaseCostImportController created
- ✅ Generic import JavaScript created
- ✅ Product.cshtml updated with import section
- ❌ PromotionCost.cshtml still has old import section
- ❌ BaseCost.cshtml needs import section added

## Issues Found

### 1. Bootstrap Version Mismatch
- **Problem**: Using `data-bs-toggle` (Bootstrap 5) but application uses Bootstrap 3/4
- **Solution**: Changed to `data-toggle` in Product.cshtml
- **Status**: ✅ Fixed for Product page

### 2. Missing Import UI Elements
- **Problem**: JavaScript trying to initialize elements that don't exist
- **Solution**: Need to add proper import sections to all pages
- **Status**: ❌ PromotionCost and BaseCost pages need UI updates

### 3. Configuration Files
- **Status**: ✅ All config files exist in wwwroot/config/

## Testing Steps

### 1. Test Product Import
1. Navigate to `/all-product`
2. Click "Import Products" button
3. Check browser console for errors
4. Try uploading an Excel file

### 2. Check API Endpoints
Test these URLs:
- `/api/ProductImport/preview`
- `/api/PromotionCostImport/preview`
- `/api/BaseCostImport/preview`

### 3. Check Configuration Loading
Test these URLs:
- `/config/product-import-columns.json`
- `/config/promotioncost-import-columns.json`
- `/config/basecost-import-columns.json`

## Common Issues

### JavaScript Errors
- Check if `createImportManager` function is defined
- Check if XLSX library is loaded
- Check if Bootstrap collapse is working

### API Errors
- Check if controllers are registered in DI
- Check if ExcelReaderService is registered
- Check if configuration files are accessible

### UI Issues
- Check if import sections are properly added to pages
- Check if Bootstrap syntax matches version being used
- Check if CSS classes are defined

## Quick Fixes

### For PromotionCost Page
The page currently has old bulk import section. Need to:
1. Replace old import section with modern collapsible section
2. Add proper form elements with correct IDs
3. Ensure JavaScript initialization matches UI elements

### For BaseCost Page
Need to add import section similar to Product page.

### For All Pages
Ensure Bootstrap syntax is consistent:
- Use `data-toggle` instead of `data-bs-toggle`
- Use `data-target` instead of `data-bs-target`
