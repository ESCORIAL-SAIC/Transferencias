using Transferencias.Resources.Values;

namespace Transferencias
{
    public static class Message
    {
        public static async Task Error(Page page, string message) =>
            await page.DisplayAlert(AppStrings.AlertErrorTitle, message, AppStrings.AlertOkButton);
        public static async Task Info(Page page, string message) =>
            await page.DisplayAlert(AppStrings.AlertInfoTitle, message, AppStrings.AlertOkButton);
        public static async Task<bool> Confirmation(Page page, string message) =>
            await page.DisplayAlert("Confirmación", message, "Confirmar", "Cancelar");
    }
}
