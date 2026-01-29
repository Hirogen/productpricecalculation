using System;

namespace ProductPriceCalculator.Services
{
    /// <summary>
    /// Service for calculating product prices with operating costs
    /// </summary>
    public class PriceCalculationService
    {
        public PriceCalculationResult Calculate(PriceCalculationInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Calculate per-unit operating cost
            double operatingCostPerUnit = 0;
            if (input.ExpectedMonthlyUnits > 0)
            {
                operatingCostPerUnit = input.TotalOperatingCosts / input.ExpectedMonthlyUnits;
            }

            // Total direct cost (base + subproducts + operating cost per unit)
            double totalDirectCost = input.BaseCost + input.SubproductsCost + operatingCostPerUnit;

            // Apply markup
            double priceAfterMarkup = totalDirectCost * (1 + input.MarkupPercent / 100);

            // Apply tax
            double finalPricePerUnit = priceAfterMarkup * (1 + input.TaxRatePercent / 100);

            // Calculate for quantity
            double totalPrice = finalPricePerUnit * input.Quantity;

            // Calculate profit
            double profitPerUnit = finalPricePerUnit - totalDirectCost;
            double totalProfit = profitPerUnit * input.Quantity;

            // Calculate for bulk items (packages)
            double costPerUnit = input.BaseCost;
            double packagePrice = finalPricePerUnit;
            
            if (input.UnitsPerPackage > 1)
            {
                packagePrice = finalPricePerUnit * input.UnitsPerPackage;
                costPerUnit = finalPricePerUnit;
            }

            return new PriceCalculationResult
            {
                BaseCost = input.BaseCost,
                SubproductsCost = input.SubproductsCost,
                OperatingCostPerUnit = operatingCostPerUnit,
                TotalDirectCost = totalDirectCost,
                MarkupAmount = priceAfterMarkup - totalDirectCost,
                PriceAfterMarkup = priceAfterMarkup,
                TaxAmount = finalPricePerUnit - priceAfterMarkup,
                FinalPricePerUnit = finalPricePerUnit,
                ProfitPerUnit = profitPerUnit,
                TotalPrice = totalPrice,
                TotalProfit = totalProfit,
                PackagePrice = packagePrice,
                CostPerUnit = costPerUnit,
                GrossMarginPercent = input.MarkupPercent / (1 + input.MarkupPercent / 100) * 100
            };
        }

        public PortfolioCalculationResult CalculatePortfolio(PortfolioCalculationInput input)
        {
            if (input == null || input.Products == null || input.Products.Count == 0)
                throw new ArgumentException("No products provided for portfolio calculation");

            var result = new PortfolioCalculationResult
            {
                TotalUnits = 0,
                TotalOperatingCosts = input.TotalOperatingCosts,
                ProductResults = new System.Collections.Generic.List<PortfolioProductResult>()
            };

            // Calculate total units
            foreach (var product in input.Products)
            {
                result.TotalUnits += product.ExpectedMonthlyUnits;
            }

            foreach (var product in input.Products)
            {
                // Calculate percentage of total units
                double percentage = result.TotalUnits > 0 
                    ? (product.ExpectedMonthlyUnits / result.TotalUnits) * 100 
                    : 0;

                // Calculate operating cost share
                double productOperatingCostShare = input.TotalOperatingCosts * (product.ExpectedMonthlyUnits / result.TotalUnits);
                double operatingCostPerUnit = product.ExpectedMonthlyUnits > 0 
                    ? productOperatingCostShare / product.ExpectedMonthlyUnits 
                    : 0;

                // Calculate final price
                double totalDirectCost = product.BaseCost + product.SubproductsCost + operatingCostPerUnit;
                double priceAfterMarkup = totalDirectCost * (1 + product.Markup / 100);
                double finalPrice = priceAfterMarkup * (1 + product.TaxRate / 100);

                result.ProductResults.Add(new PortfolioProductResult
                {
                    ProductName = product.Name,
                    ExpectedUnits = product.ExpectedMonthlyUnits,
                    Percentage = percentage,
                    OperatingCostPerUnit = operatingCostPerUnit,
                    FinalPrice = finalPrice,
                    TotalDirectCost = totalDirectCost
                });
            }

            return result;
        }
    }

    #region Input/Output Models

    public class PriceCalculationInput
    {
        public double BaseCost { get; set; }
        public double MarkupPercent { get; set; }
        public double TaxRatePercent { get; set; }
        public double SubproductsCost { get; set; }
        public double TotalOperatingCosts { get; set; }
        public double ExpectedMonthlyUnits { get; set; }
        public int Quantity { get; set; }
        public double UnitsPerPackage { get; set; }
    }

    public class PriceCalculationResult
    {
        public double BaseCost { get; set; }
        public double SubproductsCost { get; set; }
        public double OperatingCostPerUnit { get; set; }
        public double TotalDirectCost { get; set; }
        public double MarkupAmount { get; set; }
        public double PriceAfterMarkup { get; set; }
        public double TaxAmount { get; set; }
        public double FinalPricePerUnit { get; set; }
        public double ProfitPerUnit { get; set; }
        public double TotalPrice { get; set; }
        public double TotalProfit { get; set; }
        public double PackagePrice { get; set; }
        public double CostPerUnit { get; set; }
        public double GrossMarginPercent { get; set; }
    }

    public class PortfolioCalculationInput
    {
        public System.Collections.Generic.List<PortfolioProductInput> Products { get; set; }
        public double TotalOperatingCosts { get; set; }
    }

    public class PortfolioProductInput
    {
        public string Name { get; set; }
        public double ExpectedMonthlyUnits { get; set; }
        public double BaseCost { get; set; }
        public double SubproductsCost { get; set; }
        public double Markup { get; set; }
        public double TaxRate { get; set; }
    }

    public class PortfolioCalculationResult
    {
        public double TotalUnits { get; set; }
        public double TotalOperatingCosts { get; set; }
        public System.Collections.Generic.List<PortfolioProductResult> ProductResults { get; set; }
    }

    public class PortfolioProductResult
    {
        public string ProductName { get; set; }
        public double ExpectedUnits { get; set; }
        public double Percentage { get; set; }
        public double OperatingCostPerUnit { get; set; }
        public double FinalPrice { get; set; }
        public double TotalDirectCost { get; set; }
        public double ProfitPerUnit => FinalPrice - TotalDirectCost;
    }

    #endregion
}
