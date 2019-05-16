using ParkingApp.Core;
using ParkingApp.Core.Data;
using ParkingApp.Core.Data.EntityFramework;
using ParkingApp.Data.Entity;
using ParkingApp.UI.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ParkingApp.UI
{
    public partial class AboneKayit : Form
    {
        int ProcessType = 0, subscriberId = 0;
        ParkingApp parkingApp = (ParkingApp)Application.OpenForms["ParkingApp"];
        private ParkingDbContext _dbContext;
        private IEntityRepository<Subscriber> _subscriberRepository;
        private IEntityRepository<City> _cityRepository;
        private IEntityRepository<District> _districtRepository;
        public AboneKayit(int? id = null)
        {

            InitializeComponent();
            InitializeConstructor();
            InitializeComboBoxes();
            InitializeComponentRules();
            CheckAndLoadUpdateData(id);

        }

        private void CheckAndLoadUpdateData(int? id)
        {
            if (id != null)
            {
                subscriberId = (int)id;
                btnSave.Text = "Güncelle";
                ProcessType = 1;
                var subscriber = _subscriberRepository.Get(p => p.Id == id);

                txtName.Text = subscriber.Name;
                txtSurname.Text = subscriber.Surname;
                txtLicense.Text = subscriber.License;
                txtPhone.Text = subscriber.Phone;
                txtAddress.Text = subscriber.Address;
                SelectCityComboBox(subscriber.CityId);
                SelectDistrictComboBox(subscriber.DistrictId);
            }
        }

        private void SelectDistrictComboBox(int districtId)
        {
            var district = _districtRepository.Get(p => p.Id == districtId);
            // var items = cmbDistrict.Controls.
            // cmbDistrict.SelectedIndex = districtId - 1;
        }

        private void SelectCityComboBox(int cityId)
        {
            cmbCity.SelectedIndex = cityId - 1;
        }

        private void InitializeComponentRules()
        {
            txtLicense.Casing = CharacterCasing.Upper;
        }

        private void InitializeConstructor()
        {
            _dbContext = new ParkingDbContext();
            _subscriberRepository = new efRepositoryBase<Subscriber>(_dbContext);
            _cityRepository = new efRepositoryBase<City>(_dbContext);
            _districtRepository = new efRepositoryBase<District>(_dbContext);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidationSuccess())
            {

                if (ProcessType == 0)
                {
                    CreateSubscriber();
                }
                else if (ProcessType == 1)
                {
                    UpdateSubscriber();
                }

                CallInitializeSubscriberGrid();

                this.Close();
            }
        }

        private bool ValidationSuccess()
        {
            var result = true;
            if (txtName.Text.Length <= 0)
            {
                result = false;
            }
            if (txtSurname.Text.Length <= 0)
            {
                result = false;
            }
            if (txtLicense.Text.Length <= 0)
            {
                result = false;
            }
            if (txtPhone.Text.Length <= 0)
            {
                result = false;
            }
            return result;
        }

        private void CreateSubscriber()
        {
            var subscriber = GetSubscriberEntity();
            _subscriberRepository.Add(subscriber);
            _subscriberRepository.SaveChanges();
        }

        private void UpdateSubscriber()
        {
            var subscriber = GetSubscriberEntity();
            subscriber.Id = subscriberId;
            _subscriberRepository.Update(subscriber);
            _subscriberRepository.SaveChanges();
        }

        private Subscriber GetSubscriberEntity()
        {
            var subscriber = new Subscriber()
            {
                Name = txtName.Text,
                Surname = txtSurname.Text,
                License = txtLicense.Text,
                Phone = txtPhone.Text,
                CityId = ((ComboBoxItem)cmbCity.SelectedItem).Value,
                DistrictId = ((ComboBoxItem)cmbDistrict.SelectedItem).Value,
                Address = txtAddress.Text,
                InsertDate = DateTime.Now

            };
            return subscriber;
        }



        private void CallInitializeSubscriberGrid()
        {
            parkingApp.InitializeSubs(DataGridSubsSourceType.Subcriber);
        }

        private void InitializeComboBoxes()
        {
            var cities = _cityRepository.GetAll().ToList();
            foreach (var i in cities)
            {
                var item = new ComboBoxItem()
                {
                    Text = i.Name,
                    Value = i.Id
                };

                cmbCity.Items.Add(item);
            }

        }

        private void CmbCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbDistrict.Items.Clear();
            var city = (ComboBoxItem)cmbCity.SelectedItem;
            var districts = _districtRepository.GetAll(p => p.City_Id == city.Value).ToList();
            foreach (var i in districts)
            {
                var item = new ComboBoxItem()
                {
                    Text = i.Name,
                    Value = i.Id
                };

                cmbDistrict.Items.Add(item);
            }


        }

        private void TxtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }


        }

        private void AboneKayit_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
