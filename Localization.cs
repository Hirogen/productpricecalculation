using System.Collections.Generic;
using System.Globalization;

namespace ProductPriceCalculator
{
    /// <summary>
    /// Localization Manager - Supports English and German translations
    /// </summary>
    public static class Localization
    {
        private static string currentLanguage = "en";

        public static string CurrentLanguage
        {
            get => currentLanguage;
            set
            {
                currentLanguage = value;
                OnLanguageChanged?.Invoke();
            }
        }

        public static event System.Action OnLanguageChanged;

        // All translations organized by key
        private static readonly Dictionary<string, Dictionary<string, string>> Translations = new()
        {
            // Application Title
            ["AppTitle"] = new() { ["en"] = "Product Price Calculator", ["de"] = "Produktpreisrechner" },
            
            // Navigation
            ["NavWorkflows"] = new() { ["en"] = "Workflows", ["de"] = "Arbeitsabläufe" },
            ["NavCalculation"] = new() { ["en"] = "Calculation", ["de"] = "Berechnung" },
            ["NavSubproducts"] = new() { ["en"] = "Subproducts", ["de"] = "Unterprodukte" },
            ["NavOperatingCosts"] = new() { ["en"] = "Operating Costs", ["de"] = "Betriebskosten" },
            ["ButtonManageOperatingCosts"] = new() { ["en"] = "Manage Operating Costs", ["de"] = "Betriebskosten verwalten" },
            ["NavAdvancedPricing"] = new() { ["en"] = "Advanced Pricing", ["de"] = "Erweiterte Preisgestaltung" },
            ["NavSummary"] = new() { ["en"] = "Summary Report", ["de"] = "Zusammenfassungsbericht" },
            ["NavProducts"] = new() { ["en"] = "Products", ["de"] = "Produkte" },
            
            // Products and Components
            ["HeaderProducts"] = new() { ["en"] = "📦 Products (for sale)", ["de"] = "📦 Produkte (zum Verkauf)" },
            ["HeaderComponents"] = new() { ["en"] = "🔧 Components / Materials", ["de"] = "🔧 Komponenten / Materialien" },
            ["LabelIsComponent"] = new() { ["en"] = "Component (not sold separately)", ["de"] = "Komponente (nicht separat verkauft)" },
            ["ButtonNewProduct"] = new() { ["en"] = "➕ New Product", ["de"] = "➕ Neues Produkt" },
            ["ButtonNewComponent"] = new() { ["en"] = "➕ New Component", ["de"] = "➕ Neue Komponente" },
            ["HeaderComponentCalculation"] = new() { ["en"] = "Component / Material Calculation", ["de"] = "Komponenten / Materialberechnung" },
            ["InfoCreatingComponent"] = new() { ["en"] = "Creating a Component / Material - Components are materials or sub-assemblies that are used within products.", ["de"] = "Erstellen einer Komponente / eines Materials - Komponenten sind Materialien oder Unterbaugruppen, die in Produkten verwendet werden." },
            ["LabelComponentName"] = new() { ["en"] = "Component Name:", ["de"] = "Komponentenname:" },
            ["InfoComponentPricing"] = new() { ["en"] = "Components typically have 0% markup and 0% tax. Set these if your component has additional costs.", ["de"] = "Komponenten haben normalerweise 0% Aufschlag und 0% Steuer. Setzen Sie diese, wenn Ihre Komponente zusätzliche Kosten hat." },
            ["ButtonSaveComponent"] = new() { ["en"] = "Save Component", ["de"] = "Komponente speichern" },
            ["MsgEnterComponentName"] = new() { ["en"] = "Please enter a component name.", ["de"] = "Bitte geben Sie einen Komponentennamen ein." },
            ["MsgComponentSaved"] = new() { ["en"] = "Component saved successfully!", ["de"] = "Komponente erfolgreich gespeichert!" },
            
            // Convert functionality
            ["ButtonConvertToComponent"] = new() { ["en"] = "Convert to Component", ["de"] = "In Komponente umwandeln" },
            ["ButtonConvertToProduct"] = new() { ["en"] = "Convert to Product", ["de"] = "In Produkt umwandeln" },
            ["MsgConvertToComponentConfirm"] = new() { ["en"] = "Convert this product to a component? Markup and tax will be set to 0%.", ["de"] = "Dieses Produkt in eine Komponente umwandeln? Aufschlag und Steuer werden auf 0% gesetzt." },
            ["MsgConvertToProductConfirm"] = new() { ["en"] = "Convert this component to a product? Default markup (30%) and tax (8.5%) will be applied.", ["de"] = "Diese Komponente in ein Produkt umwandeln? Standard-Aufschlag (30%) und Steuer (8,5%) werden angewendet." },
            ["MsgConvertedToComponent"] = new() { ["en"] = "Successfully converted to component!", ["de"] = "Erfolgreich in Komponente umgewandelt!" },
            ["MsgConvertedToProduct"] = new() { ["en"] = "Successfully converted to product!", ["de"] = "Erfolgreich in Produkt umgewandelt!" },
            
            // Component Dialog
            ["TitleAddComponent"] = new() { ["en"] = "Add Component to Product", ["de"] = "Komponente zum Produkt hinzufügen" },
            ["ButtonAdd"] = new() { ["en"] = "Add", ["de"] = "Hinzufügen" },
            ["ButtonCancel"] = new() { ["en"] = "Cancel", ["de"] = "Abbrechen" },
            ["ButtonManageComponents"] = new() { ["en"] = "➕ Manage Components", ["de"] = "➕ Komponenten verwalten" },
            
            // Product Portfolio
            ["NavProductPortfolio"] = new() { ["en"] = "Product Portfolio", ["de"] = "Produktportfolio" },
            ["HeaderProductPortfolio"] = new() { ["en"] = "Product Portfolio - Operating Cost Distribution", ["de"] = "Produktportfolio - Betriebskostenverteilung" },
            ["InfoPortfolio"] = new() { ["en"] = "Select products to distribute operating costs across your product line based on expected sales volume.", ["de"] = "Wählen Sie Produkte aus, um Betriebskosten basierend auf dem erwarteten Verkaufsvolumen auf Ihre Produktlinie zu verteilen." },
            ["ButtonCalculatePortfolio"] = new() { ["en"] = "Calculate Portfolio Prices", ["de"] = "Portfoliopreise berechnen" },
            ["HeaderSelectedProducts"] = new() { ["en"] = "Selected Products for Portfolio", ["de"] = "Ausgewählte Produkte für Portfolio" },
            ["LabelTotalPortfolioUnits"] = new() { ["en"] = "Total Portfolio Units:", ["de"] = "Gesamte Portfolio-Einheiten:" },
            ["LabelDistributedOperatingCost"] = new() { ["en"] = "Distributed Operating Cost:", ["de"] = "Verteilte Betriebskosten:" },
            ["ColProductName"] = new() { ["en"] = "Product", ["de"] = "Produkt" },
            ["ColExpectedUnits"] = new() { ["en"] = "Expected Units", ["de"] = "Erwartete Einheiten" },
            ["ColPercentage"] = new() { ["en"] = "% of Total", ["de"] = "% vom Gesamt" },
            ["ColOperatingCostPerUnit"] = new() { ["en"] = "Op. Cost/Unit", ["de"] = "Betr.Kosten/Einh." },
            ["ColFinalPrice"] = new() { ["en"] = "Final Price", ["de"] = "Endpreis" },
            ["MsgSelectAtLeastOneProduct"] = new() { ["en"] = "Please select at least one product.", ["de"] = "Bitte wählen Sie mindestens ein Produkt aus." },
            
            // Currency Symbol
            ["CurrencySymbol"] = new() { ["en"] = "$", ["de"] = "€" }
        };

        /// <summary>
        /// Get translated text for a given key
        /// </summary>
        public static string Get(string key)
        {
            if (Translations.TryGetValue(key, out var translations))
            {
                if (translations.TryGetValue(currentLanguage, out var translation))
                {
                    return translation;
                }
                // Fallback to English if translation not found
                if (translations.TryGetValue("en", out var englishTranslation))
                {
                    return englishTranslation;
                }
            }
            return $"[{key}]"; // Return key in brackets if not found
        }

        /// <summary>
        /// Get formatted string with currency symbol
        /// </summary>
        public static string FormatCurrency(double amount, string format = "F2")
        {
            return $"{Get("CurrencySymbol")}{amount.ToString(format)}";
        }

        /// <summary>
        /// Initialize language based on system culture
        /// </summary>
        public static void InitializeFromSystemCulture()
        {
            var culture = CultureInfo.CurrentCulture;
            if (culture.TwoLetterISOLanguageName.Equals("de", System.StringComparison.OrdinalIgnoreCase))
            {
                CurrentLanguage = "de";
            }
            else
            {
                CurrentLanguage = "en";
            }
        }
    }
}
