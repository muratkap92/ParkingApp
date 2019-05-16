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
    public partial class AddRecipe : Form
    {
        int processType = 0, recipeId = 0;
        ParkingApp parkingApp = (ParkingApp)Application.OpenForms["ParkingApp"];
        private ParkingDbContext _dbContext;
        private IEntityRepository<Recipe> _recipeRepository;
        public AddRecipe(int? id = null)
        {

            InitializeComponent();
            InitializeConstructors();
            InitializeTypeComboBox();
            CheckAndLoadUpdateData(id);
        }

        private void CheckAndLoadUpdateData(int? id)
        {
            if (id != null)
            {
                recipeId = (int)id;
                btnSave.Text = "Güncelle";
                processType = 1;

                var recipe = _recipeRepository.Get(p => p.Id == id);
                txtRecipeName.Text = recipe.Name;
                txtMinimumValue.Text = recipe.MinimumValue.ToString();
                txtMaximumValue.Text = recipe.MaximumValue.ToString();
                cheStatus.Checked = recipe.Status;
                txtCost.Text = recipe.Cost.ToString();
                cmbType.SelectedIndex = recipe.Type - 1;
                cmbType.Enabled = false;
            }
        }

        private void InitializeConstructors()
        {
            _dbContext = new ParkingDbContext();
            _recipeRepository = new efRepositoryBase<Recipe>(_dbContext);
        }

        private void InitializeTypeComboBox()
        {
            var normal = new ComboBoxItem() { Text = "Normal", Value = 1 };
            var sub = new ComboBoxItem() { Text = "Abonelik", Value = 2 };

            cmbType.Items.Add(normal);
            cmbType.Items.Add(sub);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidationSucces())
            {
                if (processType == 0)
                {
                    CreateRecipe();

                }
                else if (processType == 1)
                {
                    UpdateRecipe();
                }
                CallInitializeRecipeGrid();
                this.Close();
            }

        }

        private bool ValidationSucces()
        {
            var result = true;
            if (txtCost.Text.Length <= 0)
            {
                result = false;
            }
            if (txtMaximumValue.Text.Length <= 0)
            {
                result = false;
            }
            if (txtMinimumValue.Text.Length <= 0)
            {
                result = false;
            }
            if (txtRecipeName.Text.Length <= 0)
            {
                result = false;
            }
            if (cmbType.SelectedIndex < 0)
            {
                result = false;
            }

            return result;
        }

        private void UpdateRecipe()
        {
            var recipe = GetRecipeEntity();
            recipe.Id = recipeId;
            _recipeRepository.Update(recipe);
            _recipeRepository.SaveChanges();
        }


        private void CreateRecipe()
        {
            var recipe = GetRecipeEntity();
            _recipeRepository.Add(recipe);
            _recipeRepository.SaveChanges();
        }
        private Recipe GetRecipeEntity()
        {
            var type = (ComboBoxItem)cmbType.SelectedItem;

            var recipe = new Recipe()
            {
                Cost = Convert.ToInt32(txtCost.Text),
                InsertDate = DateTime.Now,
                MaximumValue = Convert.ToInt32(txtMaximumValue.Text),
                MinimumValue = Convert.ToInt32(txtMinimumValue.Text),
                Status = cheStatus.Checked,
                Type = type.Value,
                Name = txtRecipeName.Text
            };
            return recipe;
        }

        private void CheckNumber(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

          
        }

        private void CallInitializeRecipeGrid()
        {
            parkingApp.InitializeRecipes();
        }
    }
}
