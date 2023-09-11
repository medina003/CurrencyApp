using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyConverter.Models;
using MyConverter.Services;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MyConverter.Pages
{
    public class ConverterModel : PageModel
    {
        [BindProperty]
        public string Result { get; set; }

        [BindProperty(Name = "userInput")]
        [Required(ErrorMessage = "Please enter a value.")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Please enter a valid decimal or integer value.")]
        public string UserInput { get; set; }

        [BindProperty]
        public string CurrencySelectionRight { get; set; }

        public ValCurs Response { get; set; } = new();
        private ConverterService _converterService;

        public ConverterModel(ConverterService converterService)
        {
            _converterService = converterService;
        }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                Response = await _converterService.GetConverterInfoAsync();

            }
            catch (Exception)
            {
                return RedirectToPage("/Error");
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            Response = await _converterService.GetConverterInfoAsync();
            if (double.TryParse(UserInput, out double userInputValue))
            {
                double resultValue = ConvertCurrency(userInputValue, CurrencySelectionRight);
                Result = resultValue.ToString();
            }
            return Page();
        }
        private double ConvertCurrency(double value, string toCurrency)
        {
            string toCurrencyValue = String.Empty;

            for (int i = 0; i < Response.ValTypes.Count; i++)
            {
                for (int j = 0; j < Response.ValTypes[i].Valutes.Count; j++)
                {
                    if (Response.ValTypes[i].Valutes[j].Code == toCurrency)
                    {
                        toCurrencyValue = Response.ValTypes[i].Valutes[j].Value;
                    }
                }
            }
            double toCurrencyResult;
            Double.TryParse(toCurrencyValue, NumberStyles.Float, CultureInfo.InvariantCulture, out toCurrencyResult);
            if (toCurrencyResult > 1)
            {
                return value / toCurrencyResult;
            }
            else if (toCurrencyResult < 1)
            {
                return value * toCurrencyResult;
            }
            return value;
        }
    }
}
