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
    public partial class ParkingApp : Form
    {

        private ParkingDbContext _dbContext;
        private IEntityRepository<Data.Entity.Control> _controlRepository;
        private IEntityRepository<Subscriber> _subscriberRepository;
        private IEntityRepository<Subscription> _subscriptionRepository;
        private IEntityRepository<Recipe> _recipeRepository;



        public ParkingApp()
        {
            InitializeComponent();
            InitializeConstructor();
            InitializeParkingCars();
            InitializeComponentRules();
            InitializeRecipes();
            InitializeReport();
        }

        private void InitializeReport()
        {
            InitializeReportComboBox();
            txtReportLicense.Enabled = false;

        }

        private void InitializeReportComboBox()
        {
            var reportByLicence = new ComboBoxItem() { Text = "Plakaya Göre", Value = 1 };
            var reportByMonthlyIncome = new ComboBoxItem() { Text = "Aylık Kazanç", Value = 2 };
            cmbReportType.Items.Add(reportByLicence);
            cmbReportType.Items.Add(reportByMonthlyIncome);

        }

        private void InitializeComponentRules()
        {
            radioBtnSubscribers.Checked = true;
        }

        public void InitializeRecipes()
        {
            var recipes = _recipeRepository.GetAll().ToList();
            dataGridRecipes.DataSource = recipes;
        }

        public void InitializeSubs(DataGridSubsSourceType dataGridSubsSourceType)
        {
            if (dataGridSubsSourceType == DataGridSubsSourceType.Subcriber)
            {
                var subscribers = _subscriberRepository.GetAll().ToList();
                dataGridSubscribers.DataSource = subscribers;
            }
            else if (dataGridSubsSourceType == DataGridSubsSourceType.Subscription)
            {
                var subscriptions = _subscriptionRepository.GetAll(p => p.EndDate < DateTime.Now).ToList();
                dataGridSubscribers.DataSource = subscriptions;
            }
        }

        private void InitializeParkingCars()
        {
            var controls = _controlRepository.GetAll(p => p.ExitDate == null).ToList();
            dataGridCars.DataSource = controls;
        }

        private void InitializeConstructor()
        {
            _dbContext = new ParkingDbContext();
            _controlRepository = new efRepositoryBase<Data.Entity.Control>(_dbContext);
            _subscriberRepository = new efRepositoryBase<Subscriber>(_dbContext);
            _subscriptionRepository = new efRepositoryBase<Subscription>(_dbContext);
            _recipeRepository = new efRepositoryBase<Recipe>(_dbContext);

        }

        private void BtnEnter_Click(object sender, EventArgs e)
        {
            if (txtLicense.Text != null && txtLicense.Text.Length > 0)
            {

                var control = new Data.Entity.Control()
                {
                    EnterDate = DateTime.Now,
                    InsertDate = DateTime.Now,
                    License = txtLicense.Text

                };
                _controlRepository.Add(control);
                _controlRepository.SaveChanges();
                txtLicense.Text = string.Empty;
                InitializeParkingCars();
            }

        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text != null && txtSearch.Text.Length > 0)
            {

                var selectedCar = (int)dataGridCars.SelectedRows[0].Cells[0].Value;
                var control = _controlRepository.Get(p => p.Id == selectedCar);
                control.ExitDate = DateTime.Now;
                control.Time = (control.ExitDate - control.EnterDate).Value.Hours;
                control.Cost = GetCost((control.ExitDate - control.EnterDate).Value.Hours);
                _controlRepository.Update(control);
                _controlRepository.SaveChanges();
                InitializeParkingCars();
            }
        }

        private int? GetCost(int time)
        {
            var result = 0;
            var subcriber = _subscriberRepository.Get(p => p.License == txtLicense.Text);
            if (subcriber != null)
            {
                var subscription = _subscriptionRepository.Get(p => p.SubscriberId == subcriber.Id && p.StartDate < DateTime.Now && p.EndDate > DateTime.Now);
                if (subscription != null)
                {
                    var recipe = _recipeRepository.Get(p => p.Id == subscription.RecipeId && p.Status == true);
                    result = 0;
                }
                else
                {
                    result = GetNormalCost( time);
                }
            }
            else
            {
                result = GetNormalCost(time);
            }

            return result;
        }

        private int GetNormalCost(int time)
        {
            if (time == 0) time = 1;
            var recipe = _recipeRepository.Get(p => p.Status == true && p.MinimumValue <= time && p.MaximumValue > time);
            return recipe.Cost;
        }

        private void BtnAddSubs_Click(object sender, EventArgs e)
        {
            AboneKayit formAbone = new AboneKayit();

            formAbone.Show();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var selectedItem = (int)dataGridSubscribers.SelectedRows[0].Cells[0].Value;
            if (radioBtnSubscribers.Checked)
            {
                AboneKayit formAbone = new AboneKayit(selectedItem);

                formAbone.Show();
            }
            else if (radioBtnSubscriptions.Checked)
            {
                AddSubscription addSubscription = new AddSubscription();
                addSubscription.Show();
            }

        }

        private void TxtSarchSub_TextChanged(object sender, EventArgs e)
        {
            if (radioBtnSubscribers.Checked)
            {
                var licenses = new List<Subscriber>();
                if (txtSearchSub.Text.Length > 0)
                {
                    var key = txtSearchSub.Text;
                    licenses = _subscriberRepository.GetAll(p => p.License.Contains(key)
                                                                || p.Name.Contains(key)
                                                                || p.Surname.Contains(key)
                                                                || p.Phone.Contains(key)).ToList();
                }
                else
                {
                    licenses = _subscriberRepository.GetAll().ToList();
                }
                dataGridSubscribers.DataSource = licenses;
            }
            else if (radioBtnSubscriptions.Checked)
            {
                var subcriptions = new List<Subscription>();
                if (txtSearchSub.Text.Length > 0)
                {
                    var key = txtSearchSub.Text;
                    subcriptions = _subscriptionRepository.GetAll().ToList();
                }
                else
                {
                    subcriptions = _subscriptionRepository.GetAll().ToList();
                }

                dataGridSubscribers.DataSource = subcriptions;
            }

        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            var controls = new List<Data.Entity.Control>();
            if (txtSearch.Text.Length > 0)
            {
                var key = txtSearch.Text;
                controls = _controlRepository.GetAll(p => p.License.Contains(key) && p.ExitDate == null).ToList();
            }
            else
            {
                controls = _controlRepository.GetAll().ToList();
            }
            dataGridCars.DataSource = controls;
        }

        private void BtnAddRecipe_Click(object sender, EventArgs e)
        {
            AddRecipe addRecipe = new AddRecipe();
            addRecipe.Show();
        }

        private void BtnAddSubscription_Click(object sender, EventArgs e)
        {
            AddSubscription addSubscription = new AddSubscription();
            addSubscription.Show();
        }

        private void RadioBtnSubscribers_CheckedChanged(object sender)
        {
            if (radioBtnSubscribers.Checked)
            {
                InitializeSubs(DataGridSubsSourceType.Subcriber);
            }
        }

        private void RadioBtnSubscriptions_CheckedChanged(object sender)
        {
            if (radioBtnSubscriptions.Checked)
            {
                InitializeSubs(DataGridSubsSourceType.Subscription);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var selectedItem = (int)dataGridSubscribers.SelectedRows[0].Cells[0].Value;

            if (radioBtnSubscribers.Checked)
            {
                _subscriberRepository.Delete(selectedItem);
                _subscriberRepository.SaveChanges();
                InitializeSubs(DataGridSubsSourceType.Subcriber);
            }
            else
            {
                _subscriptionRepository.Delete(selectedItem);
                _subscriptionRepository.SaveChanges();
                InitializeSubs(DataGridSubsSourceType.Subscription);

            }
        }

        private void TxtSearchRecipe_TextChanged(object sender, EventArgs e)
        {
            if (txtSearchRecipe.Text.Length > 0)
            {
                var key = txtSearchRecipe.Text;
                var recipes = _recipeRepository.GetAll(p => p.Name.Contains(key)).ToList();
                dataGridRecipes.DataSource = recipes;
            }
            else
            {
                InitializeRecipes();
            }
        }

        private void BtnRecipeDelete_Click(object sender, EventArgs e)
        {
            var selectedItem = (int)dataGridRecipes.SelectedRows[0].Cells[0].Value;
            _recipeRepository.Delete(selectedItem);
            _recipeRepository.SaveChanges();
            InitializeRecipes();

        }

        private void BtnRecipeUpdate_Click(object sender, EventArgs e)
        {
            var selectedItem = (int)dataGridRecipes.SelectedRows[0].Cells[0].Value;
            AddRecipe addRecipe = new AddRecipe(selectedItem);
            addRecipe.Show();
        }

        private void CmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var reportType = (ComboBoxItem)cmbReportType.SelectedItem;
            if (reportType.Value == 1)
            {
                txtReportLicense.Enabled = true;
            }
        }

        private void BtnReportList_Click(object sender, EventArgs e)
        {
            var reportType = (ComboBoxItem)cmbReportType.SelectedItem;
            if (reportType == null) 
            {
                MessageBox.Show("Değer Seçiniz");
                return;
            }
            if (reportType.Value == 1)
            {
                if (txtReportLicense.Text.Length > 0)
                {

                    var controlList = _controlRepository.GetAll(p => p.License == txtLicense.Text);
                    dataGridReport.DataSource = controlList;
                }
                else
                {
                    MessageBox.Show("Plaka Giriniz.");
                }
            }
            else if (reportType.Value == 2)
            {
                var lastThirty = DateTime.Now.AddDays(-30.0);
                var incomeList = new List<MonthlyIncome>();
                var controls = _controlRepository.GetAll(p => p.ExitDate != null && p.ExitDate >= lastThirty ).ToList();
                incomeList.Add(new MonthlyIncome { Name = "Normal", Cost = (int)controls.Sum(x => x.Cost) });

                var subscriptions = _subscriptionRepository.GetAll(p => p.EndDate >= lastThirty && p.IsPaid).ToList();
                var cost = 0;
                foreach (var item in subscriptions)
                {
                   cost += _recipeRepository.Get(p => p.Id == item.RecipeId).Cost;
                }


                incomeList.Add(new MonthlyIncome() { Name = "Abonelik", Cost = cost });

                dataGridReport.DataSource = incomeList;
            }
        }
    }
}
