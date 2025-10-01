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
    private BrushConverter bc = new BrushConverter();
    private MongoClient dbClient = new MongoClient("mongodb+srv://boomyf9876_db_user:pw123@prog56693f25.gqotksc.mongodb.net/?retryWrites=true&w=majority&appName=PROG56693F25");
    public MainWindow()
    {
        InitializeComponent();

        //characters.Add(new CharacterObject("Seth", "Stomp", "Body Slam", 75, 34, 65));
        //characters.Add(new CharacterObject("Curry", "Stomp", "Body Slam", 75, 34, 65));
        //characters.Add(new CharacterObject("Dad", "Stomp", "Body Slam", 75, 34, 65));

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
        listItem.Name = $"{_obj_name}{_obj_id}";
        listItem.Margin = new Thickness(10, 0, 0, 10);
        listItem.Background = (Brush)bc.ConvertFrom("#A80874");
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

            TextBox entityTextBox = new TextBox();
            entityTextBox.Width = 120;
            entityTextBox.Name = $"{_obj_name}{_obj_id}{propertyName}";

            if (propertyValue != null)
            {
                entityTextBox.Text = propertyValue.ToString();
            }

            spEntity.Children.Add(entityLabel);
            spEntity.Children.Add(entityTextBox);

            listItem.Children.Add(spEntity);
        }

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
    private void MenuDownLoad_Click(object sender, RoutedEventArgs e)
    {

    }

    private void MenuUpload_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnSpaceshipAdd_Click(object sender, RoutedEventArgs e)
    {
        PropertyInfo[] properties = typeof(SpaceshipObject).GetProperties().Skip(1).ToArray();
        SpaceshipObject spaceship = new SpaceshipObject();

        lbSpaceShip.Children.Add(InsertNewEntity(
            spaceship,
            properties,
            "Spaceship",
            spaceship.id.ToString()
        ));

        refreshScreen();
    }

    private void btnSpaceshipUpdate_Click(object sender, RoutedEventArgs e)
    {
        var database = dbClient.GetDatabase("PROG56993F25");
        var collection = database.GetCollection<SpaceshipObject>("Spaceships");

        collection.DeleteMany(new BsonDocument());
        collection.InsertMany(spaceships);

        MessageBox.Show("MongoDB Upload Complete", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private async void btnSpaceshipLoad_Click(object sender, RoutedEventArgs e)
    {
        var database = dbClient.GetDatabase("PROG56993F25");

        List<SpaceshipObject> collection = await database.GetCollection<SpaceshipObject>("Spaceships").Find(_ => true).As<SpaceshipObject>().ToListAsync();

        spaceships = collection;
        PropertyInfo[] properties = typeof(SpaceshipObject).GetProperties().Skip(1).ToArray();

        lbSpaceShip.Children.Clear();
        foreach (SpaceshipObject spaceship in spaceships)
        {
            lbSpaceShip.Children.Add(InsertNewEntity(
                spaceship,
                properties,
                "Spaceship",
                spaceship.id.ToString()
            ));
        }

        refreshScreen();

        MessageBox.Show("MongoDB Download Complete", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}