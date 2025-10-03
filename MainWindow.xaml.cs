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
    private List<SpaceshipObject> spaceships = new List<SpaceshipObject>();
    private List<SpaceshipInput> spaceshipsInput = new List<SpaceshipInput>();

    private List<PlanetObject> planets = new List<PlanetObject>();
    private List<PlanetInput> planetsInput = new List<PlanetInput>();

    private BrushConverter bc = new BrushConverter();
    private MongoClient dbClient = new MongoClient("mongodb+srv://boomyf9876_db_user:pw123@prog56693f25.gqotksc.mongodb.net/?retryWrites=true&w=majority&appName=PROG56693F25");
    private IMongoDatabase database;
    public MainWindow()
    {
        InitializeComponent();
        database = dbClient.GetDatabase("PROG56993F25");
    }
    private void MenuDownLoad_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnSpaceshipAdd_Click(object sender, RoutedEventArgs e)
    {
        PropertyInfo[] properties = typeof(SpaceshipInput).GetProperties().Skip(1).ToArray();
        SpaceshipInput spaceship = new SpaceshipInput();

        spaceshipsInput.Add(spaceship);

        wpSpaceShip.Children.Add(InsertNewEntity(
            spaceship,
            properties,
            "Spaceship",
            spaceship.id.ToString(),
            "#A7CECB",
            textBoxSpaceShip_TextChanged,
            btnSpaceshipDelete_OnClick,
            comboBox_SelectionChanged
        ));
    }

    private void btnSpaceshipUpdate_Click(object sender, RoutedEventArgs e)
    {
        PropertyInfo[] properties_ss_input = typeof(SpaceshipInput).GetProperties().Skip(1).ToArray();
        PropertyInfo[] properties_ss = typeof(SpaceshipObject).GetProperties().Skip(1).ToArray();
        var collection = database.GetCollection<SpaceshipObject>("Spaceships");
        string errorMsg = "";
        spaceships.Clear();
        foreach (var spaceship in spaceshipsInput)
        {
            SpaceshipObject newSpaceShip = new SpaceshipObject(spaceship.id);
            verify_object_input(
                properties_ss,
                properties_ss_input,
                ref errorMsg,
                ref newSpaceShip,
                spaceship
            );
            spaceships.Add(newSpaceShip);
        }

        log_upload_status_msg(collection, errorMsg, spaceships);
    }

    private async void btnSpaceshipLoad_Click(object sender, RoutedEventArgs e)
    {
        List<SpaceshipObject> collection = await database.GetCollection<SpaceshipObject>("Spaceships").Find(_ => true).As<SpaceshipObject>().ToListAsync();

        spaceshipsInput.Clear();
        foreach (SpaceshipObject _obj in collection)
        {
            spaceshipsInput.Add(new SpaceshipInput(_obj));
        }

        reloadSpaceshipList();

        MessageBox.Show("Spaceships Reload Complete", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void btnSpaceshipDelete_OnClick(object sender, RoutedEventArgs e)
    {
        remove_entity(sender, e, spaceshipsInput, typeof(SpaceshipInput).GetProperty("id"));
        reloadSpaceshipList();
    }

    private void btnPlanetAdd_Click(object sender, RoutedEventArgs e)
    {
        PropertyInfo[] properties = typeof(PlanetInput).GetProperties().Skip(1).ToArray();
        PlanetInput planet = new PlanetInput();

        planetsInput.Add(planet);

        wpPlanet.Children.Add(InsertNewEntity(
            planet,
            properties,
            "Planet",
            planet.id.ToString(),
            "#C4C6E7",
            textBoxPlanet_TextChanged,
            btnPlanetDelete_OnClick,
            comboBox_SelectionChanged
        ));
    }
    private void btnPlanetUpdate_Click(object sender, RoutedEventArgs e)
    {
        PropertyInfo[] properties_pl_input = typeof(PlanetInput).GetProperties().Skip(1).ToArray();
        PropertyInfo[] properties_pl = typeof(PlanetObject).GetProperties().Skip(1).ToArray();
        var collection = database.GetCollection<PlanetObject>("Planetary System");
        string errorMsg = "";
        planets.Clear();
        foreach (var planet in planetsInput)
        {
            PlanetObject newPlanet = new PlanetObject(planet.id);
            verify_object_input(
                properties_pl,
                properties_pl_input,
                ref errorMsg,
                ref newPlanet,
                planet
            );
            planets.Add(newPlanet);
        }

        log_upload_status_msg(collection, errorMsg, planets);
    }
    private async void btnPlanetLoad_Click(object sender, RoutedEventArgs e)
    {
        List<PlanetObject> collection = await database.GetCollection<PlanetObject>("Planetary System").Find(_ => true).As<PlanetObject>().ToListAsync();

        planetsInput.Clear();
        foreach (PlanetObject _obj in collection)
        {
            planetsInput.Add(new PlanetInput(_obj));
        }

        reloadPlanetList();

        MessageBox.Show("Spaceships Reload Complete", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void btnPlanetDelete_OnClick(object sender, RoutedEventArgs e)
    {
        remove_entity(sender, e, planetsInput, typeof(PlanetInput).GetProperty("id"));
        reloadPlanetList();
    }

    private void textBoxSpaceShip_TextChanged(object sender, RoutedEventArgs e)
    {
        textBox_TextChanged(
            sender,
            e,
            spaceshipsInput,
            typeof(SpaceshipInput).GetProperty("id"),
            typeof(SpaceshipInput),
            SpaceshipErrLabel       
        );
    }

    private void textBoxPlanet_TextChanged(object sender, RoutedEventArgs e)
    {
        textBox_TextChanged(
            sender,
            e,
            planetsInput,
            typeof(PlanetInput).GetProperty("id"),
            typeof(PlanetInput),
            PlanetErrLabel
        );
    }
    private void comboBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (!this.IsLoaded) return;
        string[] txtbxName = ((ComboBox)sender).Name.Split("NAME");
        string txtbxTxt = ((ComboBox)sender).SelectedItem.ToString();
        int index = spaceshipsInput.FindIndex(obj => obj.id.ToString() == txtbxName[0].Split("ID")[1]);

        typeof(SpaceshipInput).GetProperty(txtbxName[1]).SetValue(spaceshipsInput[index], txtbxTxt);
    }

    /* ----------------- UTIL FUNCTIONS ----------------- */
    private void reloadSpaceshipList()
    {
        PropertyInfo[] properties = typeof(SpaceshipInput).GetProperties().Skip(1).ToArray();
        wpSpaceShip.Children.Clear();
        foreach (var spaceship in spaceshipsInput)
        {
            wpSpaceShip.Children.Add(InsertNewEntity(
                spaceship,
                properties,
                "Spaceship",
                spaceship.id.ToString(),
                "#A7CECB",
                textBoxSpaceShip_TextChanged,
                btnSpaceshipDelete_OnClick,
                comboBox_SelectionChanged
            ));
        }
    }

    private void reloadPlanetList()
    {
        PropertyInfo[] properties = typeof(PlanetInput).GetProperties().Skip(1).ToArray();
        wpPlanet.Children.Clear();
        foreach (var planet in planetsInput)
        {
            wpPlanet.Children.Add(InsertNewEntity(
                planet,
                properties,
                "Planet",
                planet.id.ToString(),
                "#C4C6E7",
                textBoxPlanet_TextChanged,
                btnPlanetDelete_OnClick,
                comboBox_SelectionChanged
            ));
        }
    }

    private void verify_object_input<T>(
        PropertyInfo[] _props_obj,
        PropertyInfo[] _props_input,
        ref string _errMsg,
        ref T _data_obj,
        object _data_input
    )
    {
        for (int i = 0; i < _props_obj.Length; i++)
        {
            string propertyName = _props_input[i].Name;
            string spInputVal = (string)_props_input[i].GetValue(_data_input);
            if (string.IsNullOrEmpty(spInputVal))
            {
                _errMsg += $"•{propertyName} cannot be empty\n";
            }
            else
            {
                Type type = _props_obj[i].GetValue(_data_obj).GetType();
                if (type == typeof(Int32))
                {
                    Int32 parsedObj;
                    if (Int32.TryParse(spInputVal, out parsedObj))
                    {
                        _props_obj[i].SetValue(_data_obj, parsedObj);
                    }
                    else
                    {
                        _errMsg += $"•{propertyName} can only be of type Int32\n";
                    }
                }
                else if (type == typeof(Decimal))
                {
                    Decimal parsedObj;
                    if (Decimal.TryParse(spInputVal, out parsedObj))
                    {
                        _props_obj[i].SetValue(_data_obj, parsedObj);
                    }
                    else
                    {
                        _errMsg += $"•{propertyName} can only be of type Decimal\n";
                    }
                }
                else if (type == typeof(ShipSpecialty))
                {
                    ShipSpecialty parsedObj;
                    if (ShipSpecialty.TryParse(spInputVal, out parsedObj))
                    {
                        _props_obj[i].SetValue(_data_obj, parsedObj);
                    }
                    else
                    {
                        _errMsg += $"•{propertyName} can only be of type ShipSpecialty\n";
                    }
                }
                else
                {
                    _props_obj[i].SetValue(_data_obj, spInputVal);
                }
            }
        }
    }

    private void log_upload_status_msg<T>(IMongoCollection<T> collection, string errorMsg, List<T> _obj_arr)
    {
        if (errorMsg == "")
        {
            collection.DeleteMany(new BsonDocument());
            collection.InsertMany(_obj_arr);
            MessageBox.Show("MongoDB Upload Complete", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show(errorMsg, "MongoDB Upload Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
    private void textBox_TextChanged<T>(
        object sender,
        RoutedEventArgs e,
        List<T> _data_list,
        PropertyInfo _id_key,
        Type _data_type,
        Label _err_label
    )
    {
        if (!this.IsLoaded) return;
        string txtbxName = ((TextBox)sender).Name;
        string txtbxTxt = ((TextBox)sender).Text;
        string keyName = txtbxName.Split("NAME")[1];
        string objID = txtbxName.Split("NAME")[0].Split("ID")[1];
        PropertyInfo property = _data_type.GetProperty(keyName);
        int index = _data_list.FindIndex(obj => _id_key.GetValue(obj).ToString() == objID);

        if (string.IsNullOrEmpty(txtbxTxt))
        {
            _err_label.Content = $"{keyName} cannot be empty";
        }
        else
        {
            _err_label.Content = "";
            property.SetValue(_data_list[index], txtbxTxt);
        }
    }

    private void remove_entity<T>(object sender, RoutedEventArgs e, List<T> _data_input, PropertyInfo _id_key)
    {
        string delBtnName = ((Button)sender).Name;
        string delEntityID = delBtnName.Split("ID")[1].Split("DELETE")[0];

        int delIdx = _data_input.FindIndex(item => _id_key.GetValue(item).ToString() == delEntityID);
        _data_input.RemoveAt(delIdx);
    }
    private StackPanel InsertNewEntity(
        object _obj, PropertyInfo[] _obj_properties,
        string _obj_name, string _obj_id, string bgColor,
        TextChangedEventHandler _f_txt_onchange,
        RoutedEventHandler _f_del_entity,
        SelectionChangedEventHandler _f_cbox_onselected
    )
    {
        StackPanel listItem = new StackPanel();
        listItem.Name = $"ID{_obj_id}";
        listItem.Margin = new Thickness(10, 0, 0, 10);
        listItem.Background = (Brush)bc.ConvertFrom(bgColor);
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
            if (propertyName == "Class" && _obj_name == "Spaceship")
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
            else
            {
                TextBox entityTextBox = new TextBox();
                entityTextBox.Width = 120;
                entityTextBox.Name = $"ID{_obj_id}NAME{propertyName}";
                entityTextBox.TextChanged += _f_txt_onchange;
                if (propertyValue != null)
                {
                    entityTextBox.Text = propertyValue.ToString();
                }
                spEntity.Children.Add(entityTextBox);
            }

            listItem.Children.Add(spEntity);
        }

        Button deleteBtn = new Button();
        deleteBtn.Name = $"ID{_obj_id}DELETE";
        deleteBtn.Width = 100;
        deleteBtn.Click += _f_del_entity;
        deleteBtn.Content = "Delete";
        deleteBtn.Margin = new Thickness(0, 3, 0, 3);

        listItem.Children.Add(deleteBtn);

        return listItem;
    }
}