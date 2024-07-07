using Microsoft.AspNetCore.Components;
using SmartMeterApp.Models;

namespace SmartMeterApp.Components
{
    public partial class ChartComponent : IDisposable
    {
        [Parameter] public EventCallback<UsageDataModel> UsageDataOnclick { get; set; }
        [Parameter] public string CurrentUserId { get; set; }
        [Parameter] public List<UsageDataModel> UsageData { get; set; } = new List<UsageDataModel>();

        private int currentYear;
        private int editMonthIndex = -1;
        private Dictionary<int, List<int>> valuesByYear = new Dictionary<int, List<int>>();

        protected override void OnInitialized()
        {
            // set initial year to curent year
            currentYear = DateTime.Now.Year;

            EventAggregator.ActionRequired += HandleActionRequired;

            if (!UsageData.Any())
            {
                valuesByYear[currentYear] = Enumerable.Repeat(0, 12).ToList();
            }
            else
            {
                foreach (UsageDataModel data in UsageData)
                {
                    if (!valuesByYear.ContainsKey(data.Year))
                    {
                        valuesByYear[data.Year] = Enumerable.Repeat(0, 12).ToList();
                    }

                    int monthIndex = months.IndexOf(data.Month);

                    valuesByYear[data.Year][monthIndex] = data.Value;
                }
            }
        }

        private void LoadPreviousYearData()
        {
            currentYear--;

            if (!valuesByYear.ContainsKey(currentYear))
            {
                valuesByYear[currentYear] = Enumerable.Repeat(0, 12).ToList();
            }
        }

        private void LoadNextYearData()
        {
            currentYear++;

            if (!valuesByYear.ContainsKey(currentYear))
            {
                valuesByYear[currentYear] = Enumerable.Repeat(0, 12).ToList();
            }
        }

        private List<string> months = new List<string>()
    {
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December"
    };

        void HandleActionRequired(int value)
        {
            if (editMonthIndex != -1)
            {
                valuesByYear[currentYear][editMonthIndex] = value;
                editMonthIndex = -1;
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            EventAggregator.ActionRequired -= HandleActionRequired;
        }

        private async Task UsageDataClicked(int monthIdx, int value)
        {
            editMonthIndex = monthIdx;
            await UsageDataOnclick.InvokeAsync(new UsageDataModel() { UserId = CurrentUserId, Year = currentYear, Month = months[monthIdx], Value = value });
        }
    }
}
