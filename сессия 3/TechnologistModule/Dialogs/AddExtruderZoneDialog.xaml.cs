using System.Threading.Tasks;
using System.Windows;
using TechnologistModule.Models;
using TechnologistModule.Services;

namespace TechnologistModule.Dialogs
{
    public partial class AddExtruderZoneDialog : Window
    {
        private readonly ApiService _api;
        private readonly int _programId;

        public AddExtruderZoneDialog(ApiService api, int programId)
        {
            InitializeComponent();
            _api = api;
            _programId = programId;
            btnSave.Click += async (s, e) => await Save();
            btnCancel.Click += (s, e) => Close();
        }

        private async Task Save()
        {
            var zone = new ExtruderProgramZone
            {
                ProgramId = _programId,
                ZoneNumber = int.TryParse(txtZoneNumber.Text, out int zn) ? zn : 1,
                TargetTemperature = decimal.TryParse(txtTargetTemperature.Text, out decimal tt) ? tt : 180,
                Tolerance = decimal.TryParse(txtTolerance.Text, out decimal tol) ? tol : 5,
                TargetPressure = decimal.TryParse(txtTargetPressure.Text, out decimal tp) ? tp : 50,
                TargetSpeed = int.TryParse(txtTargetSpeed.Text, out int ts) ? ts : 150
            };

            var result = await _api.AddExtruderZoneAsync(_programId, zone);
            if (result != null)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}