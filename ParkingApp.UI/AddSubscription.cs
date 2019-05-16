using ParkingApp.Core;
using ParkingApp.Core.Data;
using ParkingApp.Core.Data.EntityFramework;
using ParkingApp.Data.Entity;
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
    public partial class AddSubscription : Form
    {
        private ParkingDbContext _dbContext;
        private IEntityRepository<Subscription> _subscriptionRepository;
        private IEntityRepository<Recipe> _recipeRepository;
        private IEntityRepository<Subscriber> _subscriberRepository;


        public AddSubscription()
        {
            InitializeComponent();
            InitializeConstructor();
            InitializeComboxer();
        }

        private void InitializeComboxer()
        {
            #region RecipeComboBox
            var recipes = _recipeRepository.GetAll(p => p.Type == 2).ToList(); //Type 2 : Abonelik Kaydı
            foreach (var item in recipes)
            {
                var comboBoxItem = new ComboBoxItem()
                {
                    Text = item.Name,
                    Value = item.Id
                };

                cmbRecipes.Items.Add(comboBoxItem);
            }

            #endregion RecipeComboBox

            #region SubscriberComboBox
            var subcribers = _subscriberRepository.GetAll().ToList();
            foreach (var item in subcribers)
            {
                var comboBoxItem = new ComboBoxItem()
                {
                    Text = item.License,
                    Value = item.Id
                };
                cmbSubcribers.Items.Add(comboBoxItem);
            }
            #endregion
        }

        private void InitializeConstructor()
        {
            _dbContext = new ParkingDbContext();
            _subscriberRepository = new efRepositoryBase<Subscriber>(_dbContext);
            _subscriptionRepository = new efRepositoryBase<Subscription>(_dbContext);
            _recipeRepository = new efRepositoryBase<Recipe>(_dbContext);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var subscriber = (ComboBoxItem)cmbSubcribers.SelectedItem;
            var recipe = (ComboBoxItem)cmbRecipes.SelectedItem;

            var subscription = new Subscription()
            {
                InsertDate = DateTime.Now,
                SubscriberId = subscriber.Value,
                RecipeId = recipe.Value,
                StartDate = datePickerStartDate.Value,
                EndDate = datePickerEndDate.Value,
                IsPaid = checkBoxPaid.Checked
            };
            _subscriptionRepository.Add(subscription);
            _subscriptionRepository.SaveChanges();
            this.Close();
        }
    }
}
