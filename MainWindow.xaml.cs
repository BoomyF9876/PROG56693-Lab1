using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Windows.Automation;
using Microsoft.Win32;
using System.IO;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;
using System.Reflection;
using System;
using System.Xml.Linq;

namespace W2CharacterEditor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<CharacterObject> characters = new List<CharacterObject>();
    private List<SpaceshipObject> spaceships = new List<SpaceshipObject>();
    private List<SpaceshipInput> spaceshipsInput = new List<SpaceshipInput>();
    private BrushConverter bc = new BrushConverter();
    private MongoClient dbClient = new MongoClient("mongodb+srv://boomyf9876_db_user:pw123@prog56693f25.gqotksc.mongodb.net/?retryWrites=true&w=majority&appName=PROG56693F25");

    public MainWindow()
    {
        InitializeComponent();
        refreshScreen();
    }

    private void refreshScreen()
    {
        dgCharacter.ItemsSource = null;
        dgCharacter.ItemsSource = characters;

        var chars = from c in characters
                    select c.Name;
        cmbCharacter.ItemsSource = chars;
    }

    private void btnCharAdd_Click(object sender, RoutedEventArgs e)
    {
        if((tbCharName.Text == "" ) || (tbCharPrimary.Text == "") || (tbCharSecondary.Text == ""))
        {
            MessageBox.Show("No textbox can be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        } else
        {
            CharacterObject c = new CharacterObject(
                tbCharName.Text,
                tbCharPrimary.Text,
                tbCharSecondary.Text,
                Math.Truncate(slCharAttack.Value),
                Math.Truncate(slCharDefense.Value),
                Math.Truncate(slCharHealth.Value));
            characters.Add(c);
            refreshScreen();
            MessageBox.Show("Add complete", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }    
    }

    private void btnCharUpdate_Click(object sender, RoutedEventArgs e)
    {
        CharacterObject c = new CharacterObject(
                tbCharName.Text,
                tbCharPrimary.Text,
                tbCharSecondary.Text,
                Math.Truncate(slCharAttack.Value),
                Math.Truncate(slCharDefense.Value),
                Math.Truncate(slCharHealth.Value));

        characters[cmbCharacter.SelectedIndex] = c;
        refreshScreen();

        MessageBox.Show("Update Complete", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void btnCharDelete_Click(object sender, RoutedEventArgs e)
    {
        characters.RemoveAt(cmbCharacter.SelectedIndex);
        refreshScreen();
        MessageBox.Show("Delete Complete", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void slCharAttack_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        lbCharAttack.Content = "Attack Strength: " + Math.Truncate(slCharAttack.Value);
    }

    private void slCharDefense_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        lbCharDefense.Content = "Defense Strength: " + Math.Truncate(slCharDefense.Value);
    }

    private void slCharHealth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        lbCharHealth.Content = "Health Strength: " + Math.Truncate(slCharHealth.Value);
    }

    private void cmbCharacter_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            int i = cmbCharacter.SelectedIndex;
            CharacterObject c = characters[i];

            tbCharName.Text = c.Name;
            tbCharPrimary.Text = c.PrimaryAttack;
            tbCharSecondary.Text = c.SecondaryAttack;
            lbCharAttack.Content = "Attack Strength: " + c.AttackStrength;
            lbCharDefense.Content = "Defense Strength: " + c.DefenseStrength;
            lbCharHealth.Content = "Health Strength: " + c.HealthStrength;
            slCharAttack.Value = c.AttackStrength;
            slCharDefense.Value = c.DefenseStrength;
            slCharHealth.Value = c.HealthStrength;
        } catch(Exception ex) { }
    } 

    private void dgCharacter_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void MenuLoad_Click(object sender, RoutedEventArgs e)
    {
        loadData();
    }

    private void MenuSave_Click(object sender, RoutedEventArgs e)
    {
        saveData();
    }

    private void MenuQuit_Click(object sender, RoutedEventArgs e)
    {

    }

    private void MenuCLoad_Click(object sender, RoutedEventArgs e)
    {
        loadData();
    }

    private void MenuCSave_Click(object sender, RoutedEventArgs e)
    {
        saveData();
    }

    private void saveData()
    {
        SaveFileDialog dlg = new SaveFileDialog();
        dlg.Title = "Select File to Save";
        dlg.Filter = "PNG File (*.png)|*.png";

        if (dlg.ShowDialog() == true)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            string jsonString = JsonSerializer.Serialize(characters, options);
            File.WriteAllText(dlg.FileName, jsonString);

            MessageBox.Show("Save Complete", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void loadData()
    {
        OpenFileDialog dlg = new OpenFileDialog();
        dlg.Title = "Select File To Load";
        dlg.Filter = "PNG File (*.png)|*.png";

        if (dlg.ShowDialog() == true)
        {
            string jsonString = File.ReadAllText(dlg.FileName);
            characters = JsonSerializer.Deserialize<List<CharacterObject>>(jsonString);
            refreshScreen();

            MessageBox.Show("Load Complete", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private StackPanel InsertNewEntity(
        object _obj, PropertyInfo[] _obj_properties,
        string _obj_name, string _obj_id
    )
    {
        StackPanel listItem = new StackPanel();
        listItem.Name = $"ID{_obj_id}";
        listItem.Margin = new Thickness(10, 0, 0, 10);
        listItem.Background = (Brush)bc.ConvertFrom("#A7CECB");
        listItem.Width = 230;
        listItem.Height = double.NaN;

        foreach (PropertyInfo property in _obj_properties)
        {
            string propertyName = property.Name;
            var propertyValue = property.GetValue(_obj);

            StackPanel spEntity = new StackPanel();
            spEntity.Margin = new Thickness(0, 3, 0, 3);
            spEntity.Orientation = Orientation.Horizontal;

            Label entityLabel = new Label();
            entityLabel.Width = 100;
            entityLabel.Content = $"{propertyName}:";

            spEntity.Children.Add(entityLabel);
            if (propertyName != "Class")
            {
                TextBox entityTextBox = new TextBox();
                entityTextBox.Width = 120;
                entityTextBox.Name = $"ID{_obj_id}NAME{propertyName}";
                entityTextBox.TextChanged += textBox_TextChanged;
                if (propertyValue != null)
                {
                    entityTextBox.Text = propertyValue.ToString();
                }
                spEntity.Children.Add(entityTextBox);
            }
            else
            {
                ComboBox entityTextBox = new ComboBox();
                entityTextBox.Width = 120;
                entityTextBox.Name = $"ID{_obj_id}NAME{propertyName}";

                foreach (ShipSpecialty sp in Enum.GetValues(typeof(ShipSpecialty)).Cast<ShipSpecialty>())
                {
                    entityTextBox.Items.Add(sp.ToString());
                }
                entityTextBox.SelectionChanged += comboBox_SelectionChanged;
                entityTextBox.SelectedItem = propertyValue;

                spEntity.Children.Add(entityTextBox);
            }

            listItem.Children.Add(spEntity);
        }

        Button deleteBtn = new Button();
        deleteBtn.Name = $"ID{_obj_id}DELETE";
        deleteBtn.Width = 100;
        deleteBtn.Click += deleteBtn_OnClick;
        deleteBtn.Content = "Delete";
        deleteBtn.Margin = new Thickness(0, 3, 0, 3);

        listItem.Children.Add(deleteBtn);

        return listItem;
    }

    private async void MenuMongoLoad_Click(object sender, RoutedEventArgs e)
    {
        var database = dbClient.GetDatabase("PROG56993F25");

        List<CharacterObject> collection = await database.GetCollection<CharacterObject>("Characters").Find(_ => true).As<CharacterObject>().ToListAsync();

        characters = collection;
        refreshScreen();

        MessageBox.Show("MongoDB Download Complete", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuMongoSave_Click(object sender, RoutedEventArgs e)
    {
        var database = dbClient.GetDatabase("PROG56993F25");
        var collection = database.GetCollection<CharacterObject>("Characters");

        collection.DeleteMany(new BsonDocument());
        collection.InsertMany(characters);

        MessageBox.Show("MongoDB Upload Complete", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void btnSpaceshipAdd_Click(object sender, RoutedEventArgs e)
    {
        PropertyInfo[] properties = typeof(SpaceshipInput).GetProperties().Skip(1).ToArray();
        SpaceshipInput spaceship = new SpaceshipInput();

        spaceshipsInput.Add(spaceship);

        lbSpaceShip.Children.Add(InsertNewEntity(
            spaceship,
            properties,
            "Spaceship",
            spaceship.id.ToString()
        ));
    }

    private void btnSpaceshipUpdate_Click(object sender, RoutedEventArgs e)
    {
        PropertyInfo[] properties_ss_input = typeof(SpaceshipInput).GetProperties().Skip(1).ToArray();
        PropertyInfo[] properties_ss = typeof(SpaceshipObject).GetProperties().Skip(1).ToArray();
        var collection = dbClient.GetDatabase("PROG56993F25").GetCollection<SpaceshipObject>("Spaceships");
        string errorMsg = "";
        spaceships.Clear();
        foreach (SpaceshipInput spaceship in spaceshipsInput)
        {
            SpaceshipObject newSpaceShip = new SpaceshipObject(spaceship.id);
            for (int i = 0; i < properties_ss.Length; i++)
            {
                string propertyName = properties_ss_input[i].Name;
                string spInputVal = (string)properties_ss_input[i].GetValue(spaceship);
                if (string.IsNullOrEmpty(spInputVal))
                {
                    errorMsg += $"•{propertyName} cannot be empty\n";
                }
                else
                {
                    Type type = properties_ss[i].GetValue(newSpaceShip).GetType();
                    if (type == typeof(Int32))
                    {
                        Int32 parsedObj;
                        if (Int32.TryParse(spInputVal, out parsedObj))
                        {
                            properties_ss[i].SetValue(newSpaceShip, parsedObj);
                        }
                        else
                        {
                            errorMsg += $"•{propertyName} can only be of type Int32\n";
                        }
                    }
                    else if (type == typeof(Decimal))
                    {
                        Decimal parsedObj;
                        if (Decimal.TryParse(spInputVal, out parsedObj))
                        {
                            properties_ss[i].SetValue(newSpaceShip, parsedObj);
                        }
                        else
                        {
                            errorMsg += $"•{propertyName} can only be of type Decimal\n";
                        }
                    }
                    else if (type == typeof(ShipSpecialty))
                    {
                        ShipSpecialty parsedObj;
                        if (ShipSpecialty.TryParse(spInputVal, out parsedObj))
                        {
                            properties_ss[i].SetValue(newSpaceShip, parsedObj);
                        }
                        else
                        {
                            errorMsg += $"•{propertyName} can only be of type ShipSpecialty\n";
                        }
                    }
                    else
                    {
                        properties_ss[i].SetValue(newSpaceShip, spInputVal);
                    }
                }
            }
            spaceships.Add(newSpaceShip);
        }
        
        if (errorMsg == "")
        {
            collection.DeleteMany(new BsonDocument());
            collection.InsertMany(spaceships);
            MessageBox.Show("MongoDB Upload Complete", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show(errorMsg, "MongoDB Upload Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void btnSpaceshipLoad_Click(object sender, RoutedEventArgs e)
    {
        var database = dbClient.GetDatabase("PROG56993F25");

        List<SpaceshipObject> collection = await database.GetCollection<SpaceshipObject>("Spaceships").Find(_ => true).As<SpaceshipObject>().ToListAsync();

        spaceshipsInput.Clear();
        foreach (SpaceshipObject _obj in collection)
        {
            spaceshipsInput.Add(new SpaceshipInput(_obj));
        }

        Reload_Spaceship_List();

        MessageBox.Show("MongoDB Download Complete", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void deleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        string delBtnName = ((Button)sender).Name;
        string delEntityID = delBtnName.Split("ID")[1].Split("DELETE")[0];

        int delIdx = spaceshipsInput.FindIndex(item => item.id.ToString() == delEntityID);
        spaceshipsInput.RemoveAt(delIdx);

        Reload_Spaceship_List();
    }

    private void Reload_Spaceship_List()
    {
        PropertyInfo[] properties = typeof(SpaceshipInput).GetProperties().Skip(1).ToArray();
        lbSpaceShip.Children.Clear();
        foreach (SpaceshipInput spaceship in spaceshipsInput)
        {
            lbSpaceShip.Children.Add(InsertNewEntity(
                spaceship,
                properties,
                "Spaceship",
                spaceship.id.ToString()
            ));
        }
    }

    private void textBox_TextChanged(object sender, RoutedEventArgs e)
    {
        if (!this.IsLoaded) return;
        string txtbxName = ((TextBox)sender).Name;
        string txtbxTxt = ((TextBox)sender).Text;
        string keyName = txtbxName.Split("NAME")[1];
        string objID = txtbxName.Split("NAME")[0].Split("ID")[1];
        int index = spaceshipsInput.FindIndex(obj => obj.id.ToString() == objID);
        PropertyInfo property = typeof(SpaceshipInput).GetProperty(keyName);

        if (string.IsNullOrEmpty(txtbxTxt))
        {
            SpaceshipErrLabel.Content = $"{keyName} cannot be empty";
        }
        else
        {
            SpaceshipErrLabel.Content = "";
            property.SetValue(spaceshipsInput[index], txtbxTxt);
        }
    }
    private void comboBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (!this.IsLoaded) return;
        string txtbxName = ((ComboBox)sender).Name;
        string txtbxTxt = ((ComboBox)sender).SelectedItem.ToString();
        string keyName = txtbxName.Split("NAME")[1];
        string objID = txtbxName.Split("NAME")[0].Split("ID")[1];
        int index = spaceshipsInput.FindIndex(obj => obj.id.ToString() == objID);

        PropertyInfo property = typeof(SpaceshipInput).GetProperty(keyName);

        if (string.IsNullOrEmpty(txtbxTxt))
        {
            SpaceshipErrLabel.Content = $"{keyName} cannot be empty";
        }
        else
        {
            SpaceshipErrLabel.Content = "";
            property.SetValue(spaceshipsInput[index], txtbxTxt);
        }
    }
}