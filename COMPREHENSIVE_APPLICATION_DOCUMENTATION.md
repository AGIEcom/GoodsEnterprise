# GoodsEnterprise Web Application - Comprehensive Documentation

## Table of Contents
1. [Application Overview](#application-overview)
2. [System Architecture](#system-architecture)
3. [Data Models & Field Specifications](#data-models--field-specifications)
4. [Page-by-Page Functionality Guide](#page-by-page-functionality-guide)
5. [Import System Documentation](#import-system-documentation)
6. [User Interface Screenshots Guide](#user-interface-screenshots-guide)
7. [Technical Specifications](#technical-specifications)
8. [User Guide](#user-guide)

---

## Application Overview

**GoodsEnterprise** is a comprehensive Enterprise Resource Planning (ERP) web application built with ASP.NET Core 3.1, designed for managing goods, suppliers, customers, and related business operations. The application features a modern, responsive UI with advanced import capabilities and comprehensive data management.

### Key Features
- **Multi-Entity Management**: Products, Suppliers, Customers, Categories, Brands, etc.
- **Advanced Import System**: Excel-based import with validation and preview
- **Modern UI**: Responsive design with modern components and interactions
- **Server-Side Pagination**: Efficient handling of large datasets
- **Role-Based Security**: Authentication and authorization system
- **Comprehensive Validation**: Field-level and business rule validation

### Technology Stack
- **Backend**: ASP.NET Core 3.1
- **Database**: SQL Server with Entity Framework Core 5.0.9
- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap
- **Architecture**: Repository Pattern with Service-Oriented Design

---

## System Architecture

### Project Structure
```
GoodsEnterprise.Web/           # Main web application
├── Pages/                     # Razor Pages
├── Controllers/               # API Controllers
├── Services/                  # Business Logic Services
├── wwwroot/                   # Static files (CSS, JS, images)
└── Configuration/             # JSON configuration files

GoosEnterprise.Model/          # Data Models
├── Models/                    # Entity models
└── CustomerModel/             # Customer-specific models

GoodsEnterprise.DataAccess/    # Data Access Layer
├── Repositories/              # Repository implementations
└── Context/                   # Database context
```

### Architecture Patterns
- **Repository Pattern**: Data access abstraction
- **Service Layer**: Business logic separation
- **Dependency Injection**: IoC container for loose coupling
- **Configuration-Driven**: JSON-based validation and settings

---

## Data Models & Field Specifications

### Product Model
**Purpose**: Core product information management

| Field Name | Data Type | Required | Description | Validation Rules |
|------------|-----------|----------|-------------|------------------|
| Id | int | Yes | Primary key | Auto-generated |
| Code | string | Yes | Product code/SKU | Unique, max 50 chars |
| ProductName | string | Yes | Product name | Max 200 chars |
| ProductDescription | string | No | Product description | Max 500 chars |
| BrandId | int? | No | Brand reference | Foreign key to Brand |
| CategoryId | int? | No | Category reference | Foreign key to Category |
| SubCategoryId | int? | No | Sub-category reference | Foreign key to SubCategory |
| InnerEan | string | No | Inner EAN barcode | Max 20 chars |
| OuterEan | string | No | Outer EAN barcode | Max 20 chars |
| UnitSize | string | No | Unit size description | Max 50 chars |
| Upc | int? | No | UPC code | Numeric |
| LayerQuantity | int? | No | Layer quantity | > 0 |
| PalletQuantity | int? | No | Pallet quantity | > 0 |
| CasePrice | decimal? | No | Case price | > 0, max 999,999.99 |
| ShelfLifeInWeeks | int? | No | Shelf life in weeks | > 0, max 520 |
| PackHeight | decimal? | No | Package height (cm) | > 0 |
| PackDepth | decimal? | No | Package depth (cm) | > 0 |
| PackWidth | decimal? | No | Package width (cm) | > 0 |
| NetCaseWeightKg | decimal? | No | Net case weight (kg) | > 0 |
| GrossCaseWeightKg | decimal? | No | Gross case weight (kg) | > 0 |
| CaseWidthMm | decimal? | No | Case width (mm) | > 0 |
| CaseHeightMm | decimal? | No | Case height (mm) | > 0 |
| CaseDepthMm | decimal? | No | Case depth (mm) | > 0 |
| PalletWeightKg | decimal? | No | Pallet weight (kg) | > 0 |
| PalletWidthMeter | decimal? | No | Pallet width (m) | > 0 |
| PalletHeightMeter | decimal? | No | Pallet height (m) | > 0 |
| PalletDepthMeter | decimal? | No | Pallet depth (m) | > 0 |
| Image | string | No | Product image URL | Valid URL format |
| SupplierId | int? | No | Supplier reference | Foreign key to Supplier |
| ExpriyDate | DateTime? | No | Expiry date | Future date |
| TaxslabId | int? | No | Tax slab reference | Foreign key to Tax |
| isTaxable | bool | No | Is taxable flag | Default: false |
| IsActive | bool | No | Active status | Default: true |
| IsDelete | bool | No | Soft delete flag | Default: false |
| CreatedDate | DateTime? | No | Creation timestamp | Auto-generated |
| Createdby | int? | No | Created by user ID | Foreign key to User |
| ModifiedDate | DateTime? | No | Last modified timestamp | Auto-updated |
| Modifiedby | int? | No | Modified by user ID | Foreign key to User |

### Supplier Model
**Purpose**: Supplier information and contact management

| Field Name | Data Type | Required | Description | Validation Rules |
|------------|-----------|----------|-------------|------------------|
| Id | int | Yes | Primary key | Auto-generated |
| Name | string | Yes | Supplier name | Max 200 chars |
| Skucode | string | No | SKU code | Max 50 chars |
| FirstName | string | No | Contact first name | Max 100 chars |
| LastName | string | No | Contact last name | Max 100 chars |
| Address1 | string | No | Primary address | Max 200 chars |
| Address2 | string | No | Secondary address | Max 200 chars |
| Phone | string | No | Phone number | Valid phone format |
| Email | string | Yes | Email address | Valid email format |
| Description | string | No | Supplier description | Max 500 chars |
| IsPreferred | bool? | No | Preferred supplier flag | Default: false |
| LeadTimeDays | int? | No | Lead time in days | > 0, max 365 |
| MoqCase | string | No | Minimum order quantity | Max 50 chars |
| LastCost | decimal? | No | Last cost | > 0 |
| Incoterm | string | No | Incoterm | Max 20 chars |
| ValidFrom | DateTime? | No | Valid from date | Valid date |
| ValidTo | DateTime? | No | Valid to date | > ValidFrom |
| IsActive | bool | No | Active status | Default: true |
| IsDelete | bool | No | Soft delete flag | Default: false |
| CreatedDate | DateTime? | No | Creation timestamp | Auto-generated |
| Createdby | int? | No | Created by user ID | Foreign key to User |
| ModifiedDate | DateTime? | No | Last modified timestamp | Auto-updated |
| Modifiedby | int? | No | Modified by user ID | Foreign key to User |

### Customer Model
**Purpose**: Customer information and account management

| Field Name | Data Type | Required | Description | Validation Rules |
|------------|-----------|----------|-------------|------------------|
| Id | int | Yes | Primary key | Auto-generated |
| FirstName | string | Yes | First name | Max 100 chars |
| LastName | string | Yes | Last name | Max 100 chars |
| Address1 | string | No | Primary address | Max 200 chars |
| Address2 | string | No | Secondary address | Max 200 chars |
| CompanyName | string | No | Company name | Max 200 chars |
| CompanyEmail | string | No | Company email | Valid email format |
| CompanyPhone | string | No | Company phone | Valid phone format |
| CompanyFax | string | No | Company fax | Valid fax format |
| ContactPerson | string | No | Contact person | Max 100 chars |
| City | string | No | City | Max 100 chars |
| County | string | No | County/State | Max 100 chars |
| PostalCode | string | No | Postal code | Valid postal format |
| Country | string | No | Country | Max 100 chars |
| MobilePhone | string | No | Mobile phone | Valid phone format |
| HomePhone | string | No | Home phone | Valid phone format |
| Email | string | Yes | Email address | Valid email format |
| Password | string | Yes | Password | Min 6 chars, strong password |
| Description | string | No | Customer description | Max 500 chars |
| RoleId | int | Yes | Role reference | Foreign key to Role |
| EmailSubscribed | bool | No | Email subscription | Default: false |
| PasswordExpiryDate | DateTime? | No | Password expiry | Future date |
| IsActive | bool | No | Active status | Default: true |
| IsDelete | bool | No | Soft delete flag | Default: false |
| CreatedDate | DateTime? | No | Creation timestamp | Auto-generated |
| Createdby | int? | No | Created by user ID | Foreign key to User |
| ModifiedDate | DateTime? | No | Last modified timestamp | Auto-updated |
| Modifiedby | int? | No | Modified by user ID | Foreign key to User |

### BaseCost Model
**Purpose**: Product base cost tracking with date ranges

| Field Name | Data Type | Required | Description | Validation Rules |
|------------|-----------|----------|-------------|------------------|
| BaseCostId | int | Yes | Primary key | Auto-generated |
| ProductId | int? | Yes | Product reference | Foreign key to Product |
| BaseCost1 | decimal? | Yes | Base cost amount | > 0, max 999,999.99 |
| StartDate | DateTime? | Yes | Start date | Valid date |
| EndDate | DateTime? | No | End date | > StartDate |
| Remark | string | No | Remarks | Max 500 chars |
| SupplierId | int? | No | Supplier reference | Foreign key to Supplier |
| IsActive | bool | No | Active status | Default: true |
| IsDelete | bool | No | Soft delete flag | Default: false |
| CreatedDate | DateTime? | No | Creation timestamp | Auto-generated |
| CreatedBy | int? | No | Created by user ID | Foreign key to User |
| ModifiedDate | DateTime? | No | Last modified timestamp | Auto-updated |
| Modifiedby | int? | No | Modified by user ID | Foreign key to User |

### PromotionCost Model
**Purpose**: Promotional pricing with time-bound offers

| Field Name | Data Type | Required | Description | Validation Rules |
|------------|-----------|----------|-------------|------------------|
| PromotionCostId | int | Yes | Primary key | Auto-generated |
| ProductId | int? | Yes | Product reference | Foreign key to Product |
| PromotionCost1 | decimal? | Yes | Promotion cost | > 0, max 999,999.99 |
| StartDate | DateTime? | Yes | Promotion start date | Valid date |
| EndDate | DateTime? | Yes | Promotion end date | > StartDate |
| Remark | string | No | Promotion remarks | Max 500 chars |
| SupplierId | int? | No | Supplier reference | Foreign key to Supplier |
| IsActive | bool | No | Active status | Default: true |
| IsDelete | bool | No | Soft delete flag | Default: false |
| CreatedDate | DateTime? | No | Creation timestamp | Auto-generated |
| CreatedBy | int? | No | Created by user ID | Foreign key to User |
| ModifiedDate | DateTime? | No | Last modified timestamp | Auto-updated |
| Modifiedby | int? | No | Modified by user ID | Foreign key to User |

---

## Page-by-Page Functionality Guide

### 1. Login Page (`/Login`)
**Purpose**: User authentication and session management

**Features**:
- Modern gradient background design
- Secure password input with visibility toggle
- Remember me functionality
- Password reset link
- Session-based authentication
- Input validation and error handling

**Fields**:
- Email: Required, valid email format
- Password: Required, minimum 6 characters
- Remember Me: Optional checkbox

**Screenshots to Capture**:
- Login form (empty state)
- Login form (with validation errors)
- Successful login redirect

### 2. Admin Management Page (`/Admin`)
**Purpose**: System administrator account management

**Features**:
- Admin user creation and editing
- Password strength validation
- Confirm password matching
- Role assignment
- Modern form controls with validation
- Toggle switches for boolean fields

**Fields**:
- First Name: Required, max 100 characters
- Last Name: Required, max 100 characters
- Email: Required, valid email format
- Password: Required, min 6 characters with strength indicator
- Confirm Password: Must match password
- Role: Dropdown selection
- Is Active: Toggle switch
- Description: Optional text area

**CRUD Operations**:
- **Create**: Add new admin user
- **Read**: View admin list with pagination
- **Update**: Edit existing admin details
- **Delete**: Soft delete with confirmation

**Screenshots to Capture**:
- Admin list view
- Add admin form
- Edit admin form
- Password validation in action
- Delete confirmation dialog

### 3. Product Management Page (`/Product`)
**Purpose**: Comprehensive product catalog management

**Features**:
- Complex product form with 20+ fields
- Image upload with preview
- Category and brand selection
- Dimension and weight specifications
- Pricing and inventory management
- Import functionality with Excel templates
- Server-side pagination for large datasets

**Field Groups**:

**Basic Information**:
- Product Code: Required, unique identifier
- Product Name: Required, display name
- Product Description: Optional, detailed description
- Brand: Dropdown selection
- Category: Dropdown selection
- Sub-Category: Dependent dropdown

**Identification Codes**:
- Inner EAN: Barcode for inner packaging
- Outer EAN: Barcode for outer packaging
- UPC: Universal Product Code

**Packaging Specifications**:
- Unit Size: Package size description
- Layer Quantity: Items per layer
- Pallet Quantity: Items per pallet
- Case Price: Price per case

**Physical Dimensions**:
- Pack Height/Width/Depth: Package dimensions in cm
- Case Height/Width/Depth: Case dimensions in mm
- Pallet Height/Width/Depth: Pallet dimensions in meters

**Weight Specifications**:
- Net Case Weight: Weight without packaging (kg)
- Gross Case Weight: Weight with packaging (kg)
- Pallet Weight: Total pallet weight (kg)

**Additional Information**:
- Shelf Life: Duration in weeks
- Image: Product photo upload
- Supplier: Associated supplier
- Tax Information: Tax slab and taxable status
- Expiry Date: Product expiry

**Import Features**:
- Excel template download
- Bulk import with validation
- Preview before import
- Error reporting and correction
- Progress tracking

**Screenshots to Capture**:
- Product list with pagination
- Add product form (all sections)
- Edit product form
- Image upload interface
- Import modal (template download)
- Import preview table
- Import progress indicator
- Import results summary

### 4. Supplier Management Page (`/Supplier`)
**Purpose**: Supplier relationship and contact management

**Features**:
- Comprehensive supplier information
- Contact details management
- Business terms and conditions
- Import functionality
- Advanced search and filtering
- Server-side pagination

**Field Groups**:

**Basic Information**:
- Supplier Name: Required, company name
- SKU Code: Supplier identification code
- First Name: Contact person first name
- Last Name: Contact person last name
- Email: Required, primary contact email

**Address Information**:
- Address 1: Primary address line
- Address 2: Secondary address line
- Phone: Contact phone number

**Business Information**:
- Description: Supplier description
- Is Preferred: Preferred supplier flag
- Lead Time Days: Delivery lead time
- MOQ Case: Minimum order quantity
- Last Cost: Last recorded cost
- Incoterm: International commercial terms
- Valid From/To: Contract validity period

**Screenshots to Capture**:
- Supplier list with search filters
- Add supplier form
- Edit supplier form
- Import supplier modal
- Search functionality in action

### 5. Customer Management Page (`/Customer`)
**Purpose**: Customer account and profile management

**Features**:
- Personal and company information
- Address management
- Account security settings
- Role-based access control
- Email subscription management

**Field Groups**:

**Personal Information**:
- First Name: Required
- Last Name: Required
- Email: Required, unique
- Password: Required with strength validation
- Confirm Password: Must match

**Company Information**:
- Company Name: Business name
- Company Email: Business email
- Company Phone: Business phone
- Company Fax: Business fax
- Contact Person: Primary contact

**Address Information**:
- Address 1 & 2: Physical address
- City, County, Postal Code: Location details
- Country: Country selection

**Contact Information**:
- Mobile Phone: Personal mobile
- Home Phone: Home contact

**Account Settings**:
- Role: User role assignment
- Email Subscribed: Newsletter subscription
- Password Expiry: Security setting

**Screenshots to Capture**:
- Customer list
- Add customer form
- Edit customer form
- Password validation interface

### 6. Category Management Page (`/Category`)
**Purpose**: Product categorization system

**Features**:
- Hierarchical category structure
- Category descriptions
- Status management
- Simple CRUD operations

**Fields**:
- Category Name: Required, unique
- Description: Optional category description
- Is Active: Status toggle

**Screenshots to Capture**:
- Category list
- Add category form
- Edit category form

### 7. Brand Management Page (`/Brand`)
**Purpose**: Brand information management

**Features**:
- Brand creation and management
- Logo upload functionality
- Status management
- Modern toggle switches

**Fields**:
- Brand Name: Required, unique
- Description: Optional brand description
- Logo: Image upload with preview
- Is Active: Status toggle switch

**Screenshots to Capture**:
- Brand list
- Add brand form with logo upload
- Edit brand form
- Logo upload interface

### 8. Base Cost Management Page (`/BaseCost`)
**Purpose**: Product base cost tracking with historical data

**Features**:
- Date-range based costing
- Product association
- Supplier linking
- Import functionality
- Historical cost analysis

**Fields**:
- Product: Required, dropdown selection
- Base Cost: Required, decimal amount
- Start Date: Required, cost effective date
- End Date: Optional, cost expiry date
- Supplier: Optional, associated supplier
- Remarks: Optional, cost notes

**Business Rules**:
- No overlapping date ranges for same product
- Start date must be before end date
- Cost must be positive value
- Historical cost tracking

**Screenshots to Capture**:
- Base cost list
- Add base cost form
- Edit base cost form
- Import base cost modal
- Date validation in action

### 9. Promotion Cost Management Page (`/PromotionCost`)
**Purpose**: Promotional pricing management

**Features**:
- Time-bound promotional pricing
- Product-specific promotions
- Supplier-linked promotions
- Import functionality
- Promotion period validation

**Fields**:
- Product: Required, dropdown selection
- Promotion Cost: Required, promotional price
- Start Date: Required, promotion start
- End Date: Required, promotion end
- Supplier: Optional, promotion sponsor
- Remarks: Optional, promotion details

**Business Rules**:
- End date must be after start date
- No overlapping promotions for same product
- Promotion cost validation
- Duration limits

**Screenshots to Capture**:
- Promotion cost list
- Add promotion form
- Edit promotion form
- Import promotion modal
- Date range validation

### 10. Tax Management Page (`/Tax`)
**Purpose**: Tax slab and rate management

**Features**:
- Tax rate configuration
- Tax slab management
- Product tax assignment

**Screenshots to Capture**:
- Tax list
- Add tax form
- Edit tax form

---

## Import System Documentation

### Overview
The GoodsEnterprise application features a sophisticated import system that allows bulk data import via Excel files with comprehensive validation, preview, and error handling.

### Supported Import Types
1. **Supplier Import**
2. **Product Import**
3. **Base Cost Import**
4. **Promotion Cost Import**

### Import Process Flow

#### 1. Template Download
- Each import type has a specific Excel template
- Templates include column headers and sample data
- Configuration-driven column definitions
- Downloadable from import modal

#### 2. File Upload & Validation
- Excel file format validation (.xlsx, .xls)
- File size limits (configurable)
- Column header validation
- Data type validation

#### 3. Preview & Validation
- Preview first 10 records
- Field-level validation errors
- Business rule validation
- Duplicate detection (batch and database)
- Warning system for data quality issues

#### 4. Import Execution
- Progress tracking with percentage completion
- Cancellation support
- Batch processing for large files
- Error logging and reporting

#### 5. Results Summary
- Success/failure statistics
- Detailed error reports
- Sample successful records
- Failed records with error reasons

### Configuration Files

#### Supplier Import Configuration (`supplier-import-columns.json`)
```json
{
  "columns": [
    {
      "name": "Name",
      "displayName": "Supplier Name",
      "required": true,
      "type": "string",
      "maxLength": 200
    },
    {
      "name": "Email",
      "displayName": "Email Address",
      "required": true,
      "type": "email"
    }
    // ... more columns
  ]
}
```

#### Product Import Configuration (`product-import-columns.json`)
- 23 total columns (2 required, 21 optional)
- Complex validation rules
- Cross-reference validation

#### Base Cost Import Configuration (`basecost-import-columns.json`)
- 8 total columns (3 required, 5 optional)
- Date range validation
- Cost amount validation

#### Promotion Cost Import Configuration (`promotioncost-import-columns.json`)
- 8 total columns (4 required, 4 optional)
- Promotion period validation
- Cost validation with business rules

### Validation Rules

#### Field-Level Validation
- **Required Fields**: Must have values
- **Data Types**: String, numeric, date, boolean, email
- **Length Limits**: Maximum character limits
- **Format Validation**: Email, phone, date formats
- **Range Validation**: Numeric min/max values

#### Business Rule Validation
- **Duplicate Detection**: Within batch and against database
- **Cross-Reference**: Validate against existing records
- **Date Logic**: Start/end date relationships
- **Business Constraints**: Cost limits, date ranges

#### Error Handling
- **Field Errors**: Specific field validation failures
- **Row Errors**: Business rule violations
- **Global Errors**: File-level issues
- **Warnings**: Data quality concerns

### Import Modal UI Features

#### Template Section
- Download template button
- Expected columns accordion
- Column descriptions and requirements

#### Upload Section
- Drag-and-drop file upload
- File format validation
- Upload progress indicator

#### Preview Section
- Validation results summary
- Sample records table
- Error and warning counts
- Duplicate detection results

#### Import Section
- Import progress bar
- Estimated completion time
- Cancel import option
- Real-time status updates

#### Results Section
- Success/failure statistics
- Detailed results tabs
- Error report download
- Sample successful records

---

## User Interface Screenshots Guide

### Recommended Screenshots for Documentation

#### 1. Authentication & Security
- [ ] Login page (empty state)
- [ ] Login page with validation errors
- [ ] Password reset page
- [ ] Session timeout message

#### 2. Dashboard & Navigation
- [ ] Main dashboard/home page
- [ ] Navigation menu (expanded)
- [ ] User profile dropdown
- [ ] Responsive mobile view

#### 3. Product Management
- [ ] Product list view with pagination
- [ ] Product list with search filters active
- [ ] Add product form (basic information section)
- [ ] Add product form (packaging specifications)
- [ ] Add product form (physical dimensions)
- [ ] Edit product form (populated fields)
- [ ] Product image upload interface
- [ ] Product validation errors
- [ ] Product delete confirmation

#### 4. Import Functionality
- [ ] Product import modal (initial state)
- [ ] Template download section
- [ ] Expected columns accordion (expanded)
- [ ] File upload area (drag-and-drop)
- [ ] File upload with validation error
- [ ] Import preview table
- [ ] Validation results summary
- [ ] Import progress indicator
- [ ] Import results (success summary)
- [ ] Import results (error details)
- [ ] Failed records tab

#### 5. Supplier Management
- [ ] Supplier list with server-side pagination
- [ ] Supplier search filters
- [ ] Add supplier form
- [ ] Edit supplier form
- [ ] Supplier import modal
- [ ] Supplier validation in action

#### 6. Customer Management
- [ ] Customer list view
- [ ] Add customer form (personal info)
- [ ] Add customer form (company info)
- [ ] Customer password validation
- [ ] Password strength indicator
- [ ] Confirm password matching

#### 7. Cost Management
- [ ] Base cost list view
- [ ] Add base cost form
- [ ] Base cost date validation
- [ ] Promotion cost list view
- [ ] Add promotion cost form
- [ ] Promotion period validation

#### 8. Category & Brand Management
- [ ] Category list view
- [ ] Add category form
- [ ] Brand list view
- [ ] Brand logo upload
- [ ] Toggle switch interactions

#### 9. Admin Features
- [ ] Admin management page
- [ ] Add admin form
- [ ] Role assignment dropdown
- [ ] Admin password validation

#### 10. Error Handling & Validation
- [ ] Form validation errors
- [ ] Server error page
- [ ] Network error message
- [ ] Loading states
- [ ] Success notifications

#### 11. Responsive Design
- [ ] Mobile view (product list)
- [ ] Tablet view (forms)
- [ ] Mobile navigation menu
- [ ] Responsive tables

#### 12. Advanced Features
- [ ] Server-side pagination controls
- [ ] Search functionality
- [ ] Filter dropdowns
- [ ] Sorting indicators
- [ ] Bulk operations
- [ ] Export functionality

### Screenshot Capture Guidelines

#### Technical Requirements
- **Resolution**: Minimum 1920x1080 for desktop views
- **Format**: PNG for UI screenshots, JPG for photos
- **Browser**: Use Chrome or Edge for consistency
- **Zoom Level**: 100% browser zoom
- **Clean State**: Clear browser cache, use incognito mode

#### Content Guidelines
- **Sample Data**: Use realistic, professional sample data
- **No Sensitive Info**: Avoid real personal or business data
- **Complete Forms**: Show forms with sample data filled in
- **Error States**: Demonstrate validation and error handling
- **Loading States**: Capture progress indicators and loading screens

#### Annotation Guidelines
- **Callouts**: Use numbered callouts for key features
- **Highlights**: Use colored boxes to highlight important areas
- **Arrows**: Point to specific UI elements
- **Captions**: Include descriptive captions for each screenshot

---

## Technical Specifications

### System Requirements
- **Operating System**: Windows Server 2016+ or Linux
- **Runtime**: .NET Core 3.1
- **Database**: SQL Server 2016+
- **Web Server**: IIS 10+ or Nginx
- **Browser Support**: Chrome 80+, Firefox 75+, Safari 13+, Edge 80+

### Performance Specifications
- **Page Load Time**: < 2 seconds for initial load
- **Search Response**: < 1 second for filtered results
- **Import Processing**: 1000 records per minute
- **Concurrent Users**: 50+ simultaneous users
- **Database**: Optimized for 100,000+ records per entity

### Security Features
- **Authentication**: Session-based authentication
- **Authorization**: Role-based access control
- **Password Policy**: Minimum 6 characters, strength validation
- **Data Protection**: Input validation and sanitization
- **HTTPS**: SSL/TLS encryption required
- **Session Management**: Configurable timeout and security

### Scalability Features
- **Server-Side Pagination**: Efficient large dataset handling
- **Lazy Loading**: On-demand data loading
- **Caching**: Application-level caching
- **Database Optimization**: Indexed queries and stored procedures

---

## User Guide

### Getting Started

#### 1. System Access
1. Navigate to the application URL
2. Enter your email and password
3. Click "Login" to access the system
4. Use "Remember Me" for convenience on trusted devices

#### 2. Navigation
- **Main Menu**: Access all modules from the navigation bar
- **Breadcrumbs**: Track your current location
- **Search**: Use global search for quick access
- **User Menu**: Access profile and logout options

### Core Operations

#### Managing Products
1. **Adding Products**:
   - Click "Add Product" button
   - Fill required fields (Code, Name)
   - Complete optional sections as needed
   - Upload product image if available
   - Save to create the product

2. **Editing Products**:
   - Find product in list or use search
   - Click "Edit" button
   - Modify required fields
   - Save changes

3. **Importing Products**:
   - Click "Import Products" button
   - Download the Excel template
   - Fill template with product data
   - Upload completed file
   - Review preview and validation
   - Execute import

#### Managing Suppliers
1. **Adding Suppliers**:
   - Navigate to Supplier page
   - Click "Add Supplier"
   - Enter supplier name and email (required)
   - Complete contact and business information
   - Save supplier record

2. **Supplier Import**:
   - Use import modal for bulk supplier creation
   - Follow template format exactly
   - Review validation results before importing

#### Cost Management
1. **Base Costs**:
   - Associate costs with products
   - Set effective date ranges
   - Link to suppliers when applicable
   - Track historical cost changes

2. **Promotion Costs**:
   - Create time-bound promotional pricing
   - Set clear start and end dates
   - Monitor promotion effectiveness

### Best Practices

#### Data Entry
- **Consistency**: Use consistent naming conventions
- **Completeness**: Fill all relevant fields for better reporting
- **Validation**: Pay attention to validation messages
- **Backup**: Regular data backups recommended

#### Import Operations
- **Template Usage**: Always use provided templates
- **Data Validation**: Validate data before import
- **Small Batches**: Import in smaller batches for better performance
- **Error Review**: Review and fix errors before re-importing

#### System Maintenance
- **Regular Updates**: Keep system updated
- **Performance Monitoring**: Monitor system performance
- **User Training**: Ensure users are properly trained
- **Data Cleanup**: Regular cleanup of inactive records

---

## Conclusion

This comprehensive documentation provides a complete guide to the GoodsEnterprise web application, covering all aspects from technical architecture to user operations. The application represents a modern, scalable solution for enterprise goods management with advanced features for data import, validation, and user experience.

For additional support or questions, please refer to the technical team or system administrator.

---

**Document Version**: 1.0  
**Last Updated**: $(Get-Date -Format "yyyy-MM-dd")  
**Prepared By**: System Documentation Team  
**Review Status**: Ready for Review
