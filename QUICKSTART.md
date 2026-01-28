# Quick Start Guide - Product Price Calculator

## 🌍 Language Support
The app automatically displays in **German** if your system is set to German, otherwise it displays in **English**. You can change the language anytime using the "Language" menu.

## Get Started in 3 Steps

### Step 1: Install .NET SDK (One-time setup)
1. Download .NET 10.0 SDK from: https://dotnet.microsoft.com/download
2. Run the installer
3. Restart your computer

### Step 2: Build the Application
1. Open the folder containing the application files
2. Double-click `build.bat`
3. Wait for the build to complete (about 30-60 seconds)

### Step 3: Run the Application
- The executable will be at: `bin\Release\net10.0-windows\win-x64\publish\ProductPriceCalculator.exe`
- Double-click to run
- No installation required!

## Alternative: Manual Build
If you prefer using the command line:
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## First Time Usage

### Calculate Your First Price
1. Launch the application
2. You'll see "Basic Calculation" selected by default
3. Enter:
   - **Base Cost**: The cost to produce/acquire the product (e.g., 50)
   - **Markup %**: Your profit margin (e.g., 30 for 30%)
   - **Tax Rate %**: Sales tax in your area (e.g., 8.5 for 8.5%)
   - **Quantity**: Number of units (e.g., 1)
4. Click "Calculate Price"
5. See your result with full breakdown!

### Example Calculation
```
Base Cost: $50.00
Markup: 30%
Tax Rate: 8.5%
Quantity: 1

Result:
- After Markup: $65.00
- After Tax: $70.53
- Final Unit Price: $70.53
```

## Explore Other Features

### Change Language
Click **Language** menu → Select **English** or **German** (Deutsch)
- Currency symbol changes automatically ($ for English, € for German)
- All text updates instantly

### Add Subproducts
Click **"Subproducts"** to add components that make up your product:
- Component name (e.g., "Packaging")
- Component cost (e.g., 5.00)
- Automatically calculates total

### Track Operating Costs
Click **"Operating Costs"** to factor in your business expenses:
- Monthly rent
- Utilities
- Labor costs
- Insurance
- Calculates cost per unit based on volume

### Advanced Pricing
Click **"Advanced Pricing"** for:
- Different pricing strategies
- Volume discounts
- Seasonal adjustments

### View Summary
Click **"Summary Report"** to see:
- Complete cost breakdown
- Profitability analysis
- Profit margins
- All calculations in one view

## Tips for Best Results

1. **Start Simple**: Begin with Basic Calculation to understand your pricing
2. **Add Detail**: Include subproducts for accurate component costs
3. **Factor Overhead**: Use Operating Costs to ensure all expenses are covered
4. **Review Regularly**: Check Summary Report to verify profitability

## Sharing the Application

To share with colleagues:
1. Copy `ProductPriceCalculator.exe` from the publish folder
2. Send via email or USB drive
3. Recipients can run it immediately (no installation!)

## Troubleshooting

**Problem**: Build script shows ".NET SDK not found"
- **Solution**: Install .NET SDK from the link above

**Problem**: Application won't start
- **Solution**: Make sure you're on Windows 7 SP1 or later

**Problem**: Numbers not calculating
- **Solution**: Enter valid numbers (use decimals like 10.5, not text)

## Need Help?

Check the README.md file for detailed documentation and customization options.

---

**You're ready to go! Happy calculating! 🎉**
