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
            
            // Menu
            ["MenuLanguage"] = new() { ["en"] = "Language", ["de"] = "Sprache" },
            ["MenuEnglish"] = new() { ["en"] = "English", ["de"] = "Englisch" },
            ["MenuGerman"] = new() { ["en"] = "German", ["de"] = "Deutsch" },
            
            // Navigation
            ["NavWorkflows"] = new() { ["en"] = "Workflows", ["de"] = "Arbeitsabläufe" },
            ["NavCalculation"] = new() { ["en"] = "Calculation", ["de"] = "Berechnung" },
            ["NavSubproducts"] = new() { ["en"] = "Subproducts", ["de"] = "Unterprodukte" },
            ["NavOperatingCosts"] = new() { ["en"] = "Operating Costs", ["de"] = "Betriebskosten" },
            ["ButtonManageOperatingCosts"] = new() { ["en"] = "Manage Operating Costs", ["de"] = "Betriebskosten verwalten" },
            ["ButtonManageOperatingCostsIcon"] = new() { ["en"] = "⚙️ Manage Operating Costs", ["de"] = "⚙️ Betriebskosten verwalten" },
            ["NavAdvancedPricing"] = new() { ["en"] = "Advanced Pricing", ["de"] = "Erweiterte Preisgestaltung" },
            ["NavSummary"] = new() { ["en"] = "Summary Report", ["de"] = "Zusammenfassungsbericht" },
            ["NavProducts"] = new() { ["en"] = "Products", ["de"] = "Produkte" },
            ["NavCategories"] = new() { ["en"] = "Categories", ["de"] = "Kategorien" },
            ["NavCompanies"] = new() { ["en"] = "Companies", ["de"] = "Firmen" },
            
            // Products and Components
            ["HeaderProducts"] = new() { ["en"] = "📦 Products (for sale)", ["de"] = "📦 Produkte (zum Verkauf)" },
            ["HeaderComponents"] = new() { ["en"] = "🔧 Components / Materials", ["de"] = "🔧 Komponenten / Materialien" },
            ["HeaderCalculation"] = new() { ["en"] = "Product Price Calculation", ["de"] = "Produktpreisberechnung" },
            ["LabelIsComponent"] = new() { ["en"] = "Component (not sold separately)", ["de"] = "Komponente (nicht separat verkauft)" },
            ["LabelProductName"] = new() { ["en"] = "Product Name:", ["de"] = "Produktname:" },
            ["ButtonNewProduct"] = new() { ["en"] = "➕ New Product", ["de"] = "➕ Neues Produkt" },
            ["ButtonLoadProduct"] = new() { ["en"] = "📂 Load Product", ["de"] = "📂 Produkt laden" },
            ["ButtonDeleteProduct"] = new() { ["en"] = "🗑️ Delete", ["de"] = "🗑️ Löschen" },
            ["ButtonNewComponent"] = new() { ["en"] = "➕ New Component", ["de"] = "➕ Neue Komponente" },
            ["ButtonLoadComponent"] = new() { ["en"] = "📂 Load Component", ["de"] = "📂 Komponente laden" },
            ["ButtonDeleteComponent"] = new() { ["en"] = "🗑️ Delete", ["de"] = "🗑️ Löschen" },
            ["HeaderComponentCalculation"] = new() { ["en"] = "Component / Material Calculation", ["de"] = "Komponenten / Materialberechnung" },
            ["InfoCreatingComponent"] = new() { ["en"] = "Creating a Component / Material - Components are materials or sub-assemblies that are used within products.", ["de"] = "Erstellen einer Komponente / eines Materials - Komponenten sind Materialien oder Unterbaugruppen, die in Produkten verwendet werden." },
            ["LabelComponentName"] = new() { ["en"] = "Component Name:", ["de"] = "Komponentenname:" },
            ["InfoComponentPricing"] = new() { ["en"] = "Components typically have 0% markup and 0% tax. Set these if your component has additional costs.", ["de"] = "Komponenten haben normalerweise 0% Aufschlag und 0% Steuer. Setzen Sie diese, wenn Ihre Komponente zusätzliche Kosten hat." },
            ["ButtonSaveComponent"] = new() { ["en"] = "Save Component", ["de"] = "Komponente speichern" },
            ["MsgEnterComponentName"] = new() { ["en"] = "Please enter a component name.", ["de"] = "Bitte geben Sie einen Komponentennamen ein." },
            ["MsgEnterProductName"] = new() { ["en"] = "Please enter a product name.", ["de"] = "Bitte geben Sie einen Produktnamen ein." },
            ["MsgComponentSaved"] = new() { ["en"] = "Component saved successfully!", ["de"] = "Komponente erfolgreich gespeichert!" },
            ["MsgProductSaved"] = new() { ["en"] = "Product saved successfully!", ["de"] = "Produkt erfolgreich gespeichert!" },
            ["MsgProductDeleted"] = new() { ["en"] = "Product deleted successfully!", ["de"] = "Produkt erfolgreich gelöscht!" },
            
            // Convert functionality
            ["ButtonConvertToComponent"] = new() { ["en"] = "Convert to Component", ["de"] = "In Komponente umwandeln" },
            ["ButtonConvertToProduct"] = new() { ["en"] = "Convert to Product", ["de"] = "In Produkt umwandeln" },
            ["MsgConvertToComponentConfirm"] = new() { ["en"] = "Convert this product to a component? Markup and tax will be set to 0%.", ["de"] = "Dieses Produkt in eine Komponente umwandeln? Aufschlag und Steuer werden auf 0% gesetzt." },
            ["MsgConvertToProductConfirm"] = new() { ["en"] = "Convert this component to a product? Default markup (30%) and tax (8.5%) will be applied.", ["de"] = "Diese Komponente in ein Produkt umwandeln? Standard-Aufschlag (30%) und Steuer (8,5%) werden angewendet." },
            ["MsgConvertedToComponent"] = new() { ["en"] = "Successfully converted to component!", ["de"] = "Erfolgreich in Komponente umgewandelt!" },
            ["MsgConvertedToProduct"] = new() { ["en"] = "Successfully converted to product!", ["de"] = "Erfolgreich in Produkt umgewandelt!" },
            
            // Labels
            ["LabelBaseCost"] = new() { ["en"] = "Base Cost:", ["de"] = "Basiskosten:" },
            ["LabelMarkup"] = new() { ["en"] = "Markup %:", ["de"] = "Aufschlag %:" },
            ["LabelTaxRate"] = new() { ["en"] = "Tax Rate %:", ["de"] = "Steuersatz %:" },
            ["LabelQuantity"] = new() { ["en"] = "Quantity:", ["de"] = "Menge:" },
            ["LabelUnitsPerPackage"] = new() { ["en"] = "Units Per Package:", ["de"] = "Einheiten pro Paket:" },
            ["LabelExpectedUnits"] = new() { ["en"] = "Expected Monthly Units:", ["de"] = "Erwartete monatliche Einheiten:" },
            ["LabelSelectProduct"] = new() { ["en"] = "Select Component:", ["de"] = "Komponente auswählen:" },
            ["LabelUnitsNeeded"] = new() { ["en"] = "Units Needed:", ["de"] = "Benötigte Einheiten:" },
            ["LabelProductCategory"] = new() { ["en"] = "Product Category:", ["de"] = "Produktkategorie:" },
            ["LabelCompany"] = new() { ["en"] = "Company/Manufacturer:", ["de"] = "Firma/Hersteller:" },
            ["LabelPurchaseLink"] = new() { ["en"] = "Purchase Link:", ["de"] = "Kauflink:" },
            
            // Categories View
            ["HeaderCategories"] = new() { ["en"] = "Product Categories", ["de"] = "Produktkategorien" },
            ["InfoCategories"] = new() { ["en"] = "Manage product categories to organize your products better.", ["de"] = "Verwalten Sie Produktkategorien, um Ihre Produkte besser zu organisieren." },
            ["HeaderAddNewCategory"] = new() { ["en"] = "➕ Add New Category", ["de"] = "➕ Neue Kategorie hinzufügen" },
            ["LabelCategoryName"] = new() { ["en"] = "Category Name:", ["de"] = "Kategoriename:" },
            ["LabelCategoryDescription"] = new() { ["en"] = "Description:", ["de"] = "Beschreibung:" },
            ["ButtonAddCategory"] = new() { ["en"] = "➕ Add Category", ["de"] = "➕ Kategorie hinzufügen" },
            ["ButtonDeleteCategory"] = new() { ["en"] = "🗑️ Delete Category", ["de"] = "🗑️ Kategorie löschen" },
            ["HeaderCurrentCategories"] = new() { ["en"] = "Current Categories", ["de"] = "Aktuelle Kategorien" },
            ["ColCategoryName"] = new() { ["en"] = "Name", ["de"] = "Name" },
            ["ColCategoryDescription"] = new() { ["en"] = "Description", ["de"] = "Beschreibung" },
            ["MsgCategoryDeleted"] = new() { ["en"] = "Category deleted successfully!", ["de"] = "Kategorie erfolgreich gelöscht!" },
            ["MsgCategorySaved"] = new() { ["en"] = "Category saved successfully!", ["de"] = "Kategorie erfolgreich gespeichert!" },
            ["MsgEnterCategoryName"] = new() { ["en"] = "Please enter a category name.", ["de"] = "Bitte geben Sie einen Kategorienamen ein." },
            
            // Companies View
            ["HeaderCompanies"] = new() { ["en"] = "Companies / Manufacturers", ["de"] = "Firmen / Hersteller" },
            ["InfoCompanies"] = new() { ["en"] = "Manage companies and manufacturers for tracking product sources.", ["de"] = "Verwalten Sie Firmen und Hersteller zur Verfolgung von Produktquellen." },
            ["HeaderAddNewCompany"] = new() { ["en"] = "➕ Add New Company", ["de"] = "➕ Neue Firma hinzufügen" },
            ["LabelCompanyName"] = new() { ["en"] = "Company Name:", ["de"] = "Firmenname:" },
            ["LabelCompanyWebsite"] = new() { ["en"] = "Website:", ["de"] = "Webseite:" },
            ["LabelCompanyContact"] = new() { ["en"] = "Contact Info:", ["de"] = "Kontaktinformationen:" },
            ["ButtonAddCompany"] = new() { ["en"] = "➕ Add Company", ["de"] = "➕ Firma hinzufügen" },
            ["ButtonDeleteCompany"] = new() { ["en"] = "🗑️ Delete Company", ["de"] = "🗑️ Firma löschen" },
            ["HeaderCurrentCompanies"] = new() { ["en"] = "Current Companies", ["de"] = "Aktuelle Firmen" },
            ["ColCompanyName"] = new() { ["en"] = "Name", ["de"] = "Name" },
            ["ColCompanyWebsite"] = new() { ["en"] = "Website", ["de"] = "Webseite" },
            ["ColCompanyContact"] = new() { ["en"] = "Contact Info", ["de"] = "Kontaktinformationen" },
            ["MsgCompanyDeleted"] = new() { ["en"] = "Company deleted successfully!", ["de"] = "Firma erfolgreich gelöscht!" },
            ["MsgCompanySaved"] = new() { ["en"] = "Company saved successfully!", ["de"] = "Firma erfolgreich gespeichert!" },
            ["MsgEnterCompanyName"] = new() { ["en"] = "Please enter a company name.", ["de"] = "Bitte geben Sie einen Firmennamen ein." },
            ["ColName"] = new() { ["en"] = "Name", ["de"] = "Name" },
            
            // Operating Costs specific labels
            ["LabelCostName"] = new() { ["en"] = "Cost Name:", ["de"] = "Kostenname:" },
            ["LabelAmount"] = new() { ["en"] = "Amount:", ["de"] = "Betrag:" },
            ["LabelIsMonthly"] = new() { ["en"] = "Monthly recurring cost", ["de"] = "Monatlich wiederkehrende Kosten" },
            ["HeaderCurrentCosts"] = new() { ["en"] = "Current Operating Costs", ["de"] = "Aktuelle Betriebskosten" },
            ["ColAmount"] = new() { ["en"] = "Amount", ["de"] = "Betrag" },
            ["ColType"] = new() { ["en"] = "Type", ["de"] = "Typ" },
            ["InfoOperatingCostsBanner"] = new() { ["en"] = "💡 Operating costs are distributed across all products based on expected sales volume. Define your monthly recurring costs here (rent, utilities, labor, etc.).", ["de"] = "💡 Betriebskosten werden basierend auf dem erwarteten Verkaufsvolumen auf alle Produkte verteilt. Definieren Sie hier Ihre monatlich wiederkehrenden Kosten (Miete, Nebenkosten, Arbeit usw.)." },
            ["HeaderAddNewOperatingCost"] = new() { ["en"] = "➕ Add New Operating Cost", ["de"] = "➕ Neue Betriebskosten hinzufügen" },
            ["HeaderAddNewCost"] = new() { ["en"] = "➕ Add New Operating Cost", ["de"] = "➕ Neue Betriebskosten hinzufügen" },
            ["LabelTotalMonthlyCosts"] = new() { ["en"] = "Total Monthly Costs:", ["de"] = "Gesamte monatliche Kosten:" },
            ["ButtonAddCost"] = new() { ["en"] = "➕ Add Cost", ["de"] = "➕ Kosten hinzufügen" },
            
            // Info messages
            ["InfoBulkPricing"] = new() { ["en"] = "For bulk items: if you buy a package of 100 units, enter 100. The cost per unit will be calculated automatically.", ["de"] = "Für Großbestellungen: Wenn Sie ein Paket mit 100 Einheiten kaufen, geben Sie 100 ein. Die Kosten pro Einheit werden automatisch berechnet." },
            ["InfoOperatingCosts"] = new() { ["en"] = "Total Monthly Operating Costs:", ["de"] = "Gesamte monatliche Betriebskosten:" },
            ["InfoExpectedUnits"] = new() { ["en"] = "Expected Monthly Units:", ["de"] = "Erwartete monatliche Einheiten:" },
            ["InfoCostPerUnit"] = new() { ["en"] = "Cost Per Unit:", ["de"] = "Kosten pro Einheit:" },
            ["InfoCostAddedBeforeMarkup"] = new() { ["en"] = "This cost is added to the base cost before markup.", ["de"] = "Diese Kosten werden vor dem Aufschlag zu den Basiskosten addiert." },
            ["InfoExpectedMonthlyUnits"] = new() { ["en"] = "Expected Monthly Units:", ["de"] = "Erwartete monatliche Einheiten:" },
            ["InfoMonthlyProfit"] = new() { ["en"] = "Expected monthly profit ({0} units):", ["de"] = "Erwarteter monatlicher Gewinn ({0} Einheiten):" },
            
            // Component Dialog
            ["TitleAddComponent"] = new() { ["en"] = "Add Component to Product", ["de"] = "Komponente zum Produkt hinzufügen" },
            ["ButtonAdd"] = new() { ["en"] = "Add", ["de"] = "Hinzufügen" },
            ["ButtonCancel"] = new() { ["en"] = "Cancel", ["de"] = "Abbrechen" },
            ["ButtonManageComponents"] = new() { ["en"] = "➕ Manage Components", ["de"] = "➕ Komponenten verwalten" },
            ["ButtonDeleteCost"] = new() { ["en"] = "🗑️ Delete", ["de"] = "🗑️ Löschen" },
            
            // Product Portfolio
            ["NavProductPortfolio"] = new() { ["en"] = "Product Portfolio", ["de"] = "Produktportfolio" },
            ["HeaderProductPortfolio"] = new() { ["en"] = "Product Portfolio - Operating Cost Distribution", ["de"] = "Produktportfolio - Betriebskostenverteilung" },
            ["InfoPortfolio"] = new() { ["en"] = "Select products to distribute operating costs across your product line based on expected sales volume.", ["de"] = "Wählen Sie Produkte aus, um Betriebskosten basierend auf dem erwarteten Verkaufsvolumen auf Ihre Produktlinie zu verteilen." },
            ["HeaderProductSelection"] = new() { ["en"] = "📦 Product Selection", ["de"] = "📦 Produktauswahl" },
            ["InfoSelectProducts"] = new() { ["en"] = "Select products to include in the portfolio analysis. Operating costs will be distributed proportionally based on expected monthly sales.", ["de"] = "Wählen Sie Produkte für die Portfolioanalyse aus. Betriebskosten werden proportional basierend auf erwarteten monatlichen Verkäufen verteilt." },
            ["ColSelected"] = new() { ["en"] = "Selected", ["de"] = "Ausgewählt" },
            ["ColProduct"] = new() { ["en"] = "Product", ["de"] = "Produkt" },
            ["ColBaseCost"] = new() { ["en"] = "Base Cost", ["de"] = "Basiskosten" },
            ["ColMarkup"] = new() { ["en"] = "Markup %", ["de"] = "Aufschlag %" },
            ["ButtonCalculatePortfolio"] = new() { ["en"] = "Calculate Portfolio Prices", ["de"] = "Portfoliopreise berechnen" },
            ["HeaderSelectedProducts"] = new() { ["en"] = "Selected Products for Portfolio", ["de"] = "Ausgewählte Produkte für Portfolio" },
            ["HeaderPortfolioAnalysis"] = new() { ["en"] = "📊 Portfolio Analysis Results", ["de"] = "📊 Portfolio-Analyseergebnisse" },
            ["LabelTotalPortfolioUnits"] = new() { ["en"] = "Total Portfolio Units:", ["de"] = "Gesamte Portfolio-Einheiten:" },
            ["LabelDistributedOperatingCost"] = new() { ["en"] = "Distributed Operating Cost:", ["de"] = "Verteilte Betriebskosten:" },
            ["ColProductName"] = new() { ["en"] = "Product", ["de"] = "Produkt" },
            ["ColExpectedUnits"] = new() { ["en"] = "Expected Units", ["de"] = "Erwartete Einheiten" },
            ["ColPercentage"] = new() { ["en"] = "% of Total", ["de"] = "% vom Gesamt" },
            ["ColOperatingCostPerUnit"] = new() { ["en"] = "Op. Cost/Unit", ["de"] = "Betr.Kosten/Einh." },
            ["ColFinalPrice"] = new() { ["en"] = "Final Price", ["de"] = "Endpreis" },
            ["ColProfit"] = new() { ["en"] = "Profit/Unit", ["de"] = "Gewinn/Einh." },
            ["MsgSelectAtLeastOneProduct"] = new() { ["en"] = "Please select at least one product.", ["de"] = "Bitte wählen Sie mindestens ein Produkt aus." },
            
            // Results
            ["ResultUnitCalc"] = new() { ["en"] = "Unit Price Calculation", ["de"] = "Stückpreisberechnung" },
            ["ResultCostBreakdown"] = new() { ["en"] = "Cost Breakdown", ["de"] = "Kostenaufschlüsselung" },
            ["ResultBaseCost"] = new() { ["en"] = "Base Cost:", ["de"] = "Basiskosten:" },
            ["ResultOperatingCostPerUnit"] = new() { ["en"] = "Operating Cost per Unit:", ["de"] = "Betriebskosten pro Einheit:" },
            ["ResultTotal"] = new() { ["en"] = "Total:", ["de"] = "Gesamt:" },
            ["ResultPricing"] = new() { ["en"] = "Pricing", ["de"] = "Preisgestaltung" },
            ["ResultAfterMarkup"] = new() { ["en"] = "After Markup", ["de"] = "Nach Aufschlag" },
            ["ResultAfterTax"] = new() { ["en"] = "After Tax", ["de"] = "Nach Steuern" },
            ["ResultSummary"] = new() { ["en"] = "Summary", ["de"] = "Zusammenfassung" },
            ["ResultFinalUnitPrice"] = new() { ["en"] = "Final Unit Price:", ["de"] = "Endpreis pro Einheit:" },
            ["ResultProfitPerUnit"] = new() { ["en"] = "Profit per Unit:", ["de"] = "Gewinn pro Einheit:" },
            ["ResultQuantity"] = new() { ["en"] = "Quantity:", ["de"] = "Menge:" },
            ["ResultTotalPrice"] = new() { ["en"] = "Total Price:", ["de"] = "Gesamtpreis:" },
            ["ResultTotalProfit"] = new() { ["en"] = "Total Profit:", ["de"] = "Gesamtgewinn:" },
            
            // Summary
            ["HeaderSummary"] = new() { ["en"] = "Summary Report", ["de"] = "Zusammenfassungsbericht" },
            ["SummaryCostBreakdown"] = new() { ["en"] = "Cost Breakdown", ["de"] = "Kostenaufschlüsselung" },
            ["SummaryBaseCost"] = new() { ["en"] = "Base Cost:", ["de"] = "Basiskosten:" },
            ["SummarySubproductsTotal"] = new() { ["en"] = "Subproducts Total:", ["de"] = "Unterprodukte gesamt:" },
            ["SummaryTotalDirectCost"] = new() { ["en"] = "Total Direct Cost:", ["de"] = "Gesamte direkte Kosten:" },
            ["SummaryPricing"] = new() { ["en"] = "Pricing", ["de"] = "Preisgestaltung" },
            ["SummaryMarkup"] = new() { ["en"] = "Markup", ["de"] = "Aufschlag" },
            ["SummaryPriceBeforeTax"] = new() { ["en"] = "Price Before Tax:", ["de"] = "Preis vor Steuern:" },
            ["SummaryTax"] = new() { ["en"] = "Tax", ["de"] = "Steuer" },
            ["SummaryFinalPrice"] = new() { ["en"] = "Final Price:", ["de"] = "Endpreis:" },
            ["SummaryOperatingCosts"] = new() { ["en"] = "Operating Costs", ["de"] = "Betriebskosten" },
            ["SummaryMonthlyOperating"] = new() { ["en"] = "Monthly Operating Costs:", ["de"] = "Monatliche Betriebskosten:" },
            ["SummaryOperatingPerUnit"] = new() { ["en"] = "Operating Cost per Unit:", ["de"] = "Betriebskosten pro Einheit:" },
            ["SummaryProfitability"] = new() { ["en"] = "Profitability", ["de"] = "Rentabilität" },
            ["SummaryGrossMargin"] = new() { ["en"] = "Gross Margin:", ["de"] = "Bruttomarge:" },
            ["SummaryProfitPerUnit"] = new() { ["en"] = "Profit per Unit:", ["de"] = "Gewinn pro Einheit:" },
            ["HeaderAddSubproducts"] = new() { ["en"] = "Add Components/Subproducts", ["de"] = "Komponenten/Unterprodukte hinzufügen" },
            ["ButtonExportReport"] = new() { ["en"] = "📄 Export Report", ["de"] = "📄 Bericht экспортieren" },
            
            // Messages
            ["MsgInvalidInput"] = new() { ["en"] = "Please enter valid values.", ["de"] = "Bitte geben Sie gültige Werte ein." },
            ["MsgInvalidTitle"] = new() { ["en"] = "Invalid Input", ["de"] = "Ungültige Eingabe" },
            ["MsgCalcError"] = new() { ["en"] = "Calculation error:", ["de"] = "Berechnungsfehler:" },
            ["MsgCalcErrorTitle"] = new() { ["en"] = "Error", ["de"] = "Fehler" },
            ["MsgDeleteConfirm"] = new() { ["en"] = "Are you sure you want to delete this item?", ["de"] = "Sind Sie sicher, dass Sie dieses Element löschen möchten?" },
            ["MsgConfirmation"] = new() { ["en"] = "Confirmation", ["de"] = "Bestätigung" },
            ["MsgSelectProduct"] = new() { ["en"] = "Please select a component and enter a valid quantity.", ["de"] = "Bitte wählen Sie eine Komponenten aus und geben Sie eine gültige Menge ein." },
            ["MsgExportReport"] = new() { ["en"] = "Report exported successfully!", ["de"] = "Bericht erfolgreich exportiert!" },
            ["MsgExport"] = new() { ["en"] = "Export", ["de"] = "Exportieren" },
            
            // Currency Symbol
            ["CurrencySymbol"] = new() { ["en"] = "$", ["de"] = "€" },
            ["ButtonShowDetails"] = new() { ["en"] = "📊 Show Details", ["de"] = "📊 Details anzeigen" },
            ["HeaderPriceCalculationResult"] = new() { ["en"] = "💰 Price Calculation Result", ["de"] = "💰 Preisberechnungsergebnis" },
            ["HeaderDetailedCalculationBreakdown"] = new() { ["en"] = "📊 Detailed Calculation Breakdown", ["de"] = "📊 Detaillierte Berechnungsaufschlüsselung" },
            ["LabelCalculation"] = new() { ["en"] = "Calculation", ["de"] = "Berechnung" },
            ["InfoCreatingComponentBanner"] = new() { ["en"] = "🔧 Creating a Component / Material - Components are materials or sub-assemblies that are used within products.", ["de"] = "🔧 Erstellen einer Komponente / eines Materials - Komponenten sind Materialien oder Unterbaugruppen, die in Produkten verwendet werden." },
            ["LabelTotalPriceFormat"] = new() { ["en"] = "Total Price (×{0}):", ["de"] = "Gesamtpreis (×{0}):" },
            ["LabelSubproductsTotal"] = new() { ["en"] = "+ Subproducts Total:", ["de"] = "+ Unterprodukte gesamt:" },
            ["LabelTotalDirectCost"] = new() { ["en"] = "= Total Direct Cost:", ["de"] = "= Gesamte direkte Kosten:" },
            ["LabelFinalPricePerUnit"] = new() { ["en"] = "Final Price per Unit:", ["de"] = "Endpreis pro Einheit:" },
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
