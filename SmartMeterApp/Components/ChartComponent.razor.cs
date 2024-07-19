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
            // 1. set initial year to curent year
            currentYear = DateTime.Now.Year;

            EventAggregator.ActionRequired += HandleActionRequired;

            // 2. Check if there are any data stored for user
            // no: show graph with empty values
            if (!UsageData.Any())
            {
                valuesByYear[currentYear] = Enumerable.Repeat(0, 12).ToList();
            }
            //yes: show graph with stored values
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

        //name of months
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

        /// <summary>
        /// This method notifies the component to update the graph data for specific month
        /// </summary>
        /// <param name="value"></param>
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

        /// <summary>
        /// This method allows the user editing values
        /// </summary>
        /// <param name="monthIdx"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private async Task UsageDataClicked(int monthIdx, int value)
        {
            editMonthIndex = monthIdx;
            await UsageDataOnclick.InvokeAsync(new UsageDataModel() { UserId = CurrentUserId, Year = currentYear, Month = months[monthIdx], Value = value });
        }
    }
}
