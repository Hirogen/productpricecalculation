# Product Price Calculator - WPF Desktop Application

A comprehensive desktop application for calculating product prices based on various factors including rent, taxes, subproducts, and operating costs.

## 🌍 Language Support

The application automatically detects your system language and displays in:
- **English** (Default)
- **German** (Deutsch)

You can switch languages anytime using the Language menu at the top of the application.

## Features

### 1. **Products Management with Database** 🗄️
- Save and load multiple products
- SQLite database stored in your AppData folder
- Track product history with creation and modification dates
- No installation or server required

### 2. **Basic Calculation Workflow**
- Calculate product prices with base cost, markup percentage, and tax rate
- Support for quantity-based pricing
- Real-time price calculation display

### 3. **Enhanced Subproducts Management**
- Add multiple subproducts with individual costs
- **Description field** for detailed notes
- **Individual tax rates** per subproduct
- Automatic total calculation
- Dynamic list management

### 4. **Custom Operating Costs** ✨
- **Add your own cost categories** (rent, utilities, labor, insurance, etc.)
- Mark costs as monthly or one-time
- Flexible cost tracking
- Delete costs individually
- Automatic monthly total calculation

### 5. **Advanced Pricing Strategy**
- Multiple pricing strategies (Cost-Plus, Competitive, Value-Based)
- Volume discount configuration
- Seasonal pricing adjustments

### 6. **Summary Report**
- Comprehensive cost breakdown
- Profitability analysis
- Gross profit margin calculation
- Export functionality (ready for implementation)

## Building the Application

### Prerequisites
- .NET 10.0 SDK or later (Download from: https://dotnet.microsoft.com/download)
- Windows OS

### Build Instructions

#### Option 1: Build for Development (Requires .NET Runtime)
```bash
dotnet build
dotnet run
```

#### Option 2: Build Standalone Executable (NO INSTALLATION REQUIRED)
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The standalone executable will be created at:
`bin/Release/net10.0-windows/win-x64/publish/ProductPriceCalculator.exe`

This executable:
- ✅ Runs without .NET installation
- ✅ Single file deployment
- ✅ Can be copied and run on any Windows PC
- ✅ No dependencies required

#### Option 3: Quick Build Script
On Windows, you can create a `build.bat` file:
```batch
@echo off
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
echo.
echo Build complete! Executable located at:
echo bin\Release\net10.0-windows\win-x64\publish\ProductPriceCalculator.exe
pause
```

## Running the Application

### After Building
1. Navigate to the publish folder
2. Double-click `ProductPriceCalculator.exe`
3. The application will start immediately

### Distributing
Simply copy the `ProductPriceCalculator.exe` file to any Windows computer and run it - no installation needed!

## Application Structure

```
ProductPriceCalculator/
├── ProductPriceCalculator.xaml.cs    # Main application code
├── Localization.cs                   # English/German translations
├── DatabaseManager.cs                # SQLite database management
├── ProductPriceCalculator.csproj     # Project configuration
└── README.md                         # This file
```

## Database Location

The SQLite database is automatically created at:
- Windows: `C:\Users\[YourName]\AppData\Local\ProductPriceCalculator\products.db`

This ensures your data persists between sessions and is safely stored in your user profile.

## Usage Guide

### Basic Calculation
1. Click "Basic Calculation" in the navigation panel
2. Enter your base cost
3. Set markup percentage (profit margin)
4. Set tax rate
5. Enter quantity
6. Click "Calculate Price" to see results

### Managing Subproducts
1. Click "Subproducts" in navigation
2. Enter subproduct name and cost
3. Click "Add Subproduct"
4. View total subproducts cost automatically

### Operating Costs
1. Click "Operating Costs"
2. Enter monthly expenses (rent, utilities, labor, insurance, etc.)
3. Enter expected monthly units
4. Calculate cost per unit

### Advanced Pricing
1. Select pricing strategy
2. Configure volume discounts
3. Apply seasonal adjustments
4. Use these settings for strategic pricing

### Summary Report
1. Click "Summary Report"
2. View comprehensive breakdown of all costs
3. See profitability metrics
4. Export report (functionality ready for implementation)

## Technical Details

- **Framework**: WPF (Windows Presentation Foundation)
- **Language**: C# 10.0
- **Target**: .NET 10.0 Windows
- **Architecture**: MVVM pattern
- **UI Components**: Custom styled native WPF controls
- **Localization**: English and German (auto-detects system language)

## Customization

The application can be easily extended:
- Add new workflows by creating additional view methods
- Modify calculation formulas in the respective methods
- Add export functionality in the `ShowSummaryReport` method
- Customize colors and styling in the UI creation code
- **Add new languages**: Edit `Localization.cs` to add more language translations

### Adding a New Language

To add a new language (e.g., French):
1. Open `Localization.cs`
2. Add "fr" translations to each dictionary entry
3. The system will automatically detect French systems

Example:
```csharp
["AppTitle"] = new() { 
    ["en"] = "Product Price Calculator", 
    ["de"] = "Produktpreisrechner",
    ["fr"] = "Calculateur de Prix de Produit"  // Add this
}
```

## System Requirements

- Windows 7 SP1 or later
- 100 MB disk space
- 512 MB RAM (recommended: 1 GB)

## License

This application is provided as-is for commercial or personal use.

## Support

For issues or questions, please refer to the code comments or modify as needed for your specific requirements.
