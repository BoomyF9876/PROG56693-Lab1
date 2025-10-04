using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MongoDB.Driver;
using System.Reflection;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Windows.Media.Animation;
using System.Drawing;
using MongoDB.Driver.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Documents;

namespace W2CharacterEditor;

public partial class MainWindow : Window
{
    private List<SpaceshipObject> spaceships;
    private List<SpaceshipInput> spaceshipsInput;

    private List<OfficerObject> officers;
    private List<OfficerInput> officersInput;

    private List<PlanetObject> planets;
    private List<PlanetInput> planetsInput;

    private List<MissionObject> missions;

    private List<string> shipSpecialties;
    private List<string> planetSystems;

    private BrushConverter bc = new BrushConverter();
    private MongoClient dbClient;
    private IMongoDatabase database;
    public MainWindow()
    {
        dbClient = new MongoClient("mongodb+srv://boomyf9876_db_user:pw123@prog56693f25.gqotksc.mongodb.net/?retryWrites=true&w=majority&appName=PROG56693F25");
        database = dbClient.GetDatabase("PROG56993F25");
        InitializeComponent();
        btnSpaceshipLoad_Click(null, null); // Calling load here since spaceship tab is used as the default tab
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        ((Window)sender).SizeToContent = SizeToContent.Height;
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    // Reload collection when user switch to a new tab
    {
        if (!this.IsLoaded || !(e.Source is TabControl tabControl)) return;
        
        string header = ((TabItem)tabControl.SelectedItem).Header.ToString();
        switch (header)
        {
            case "Space ships":
                btnSpaceshipLoad_Click(null, null);
                break;
            case "Officers":
                btnOfficerLoad_Click(null, null);
                break;
            case "Planetary Systems":
                btnPlanetLoad_Click(null, null);
                break;
            case "Missions":
                btnMissionLoad_Click(null, null);
                break;
            default:
                break;
        }
    }

    /* ----------------- SPACESHIP TAB ----------------- */
    private void btnSpaceshipAdd_Click(object sender, RoutedEventArgs e)
    {
        if (spaceshipsInput is null) return; // prevent button click while waiting for the async load function
        SpaceshipInput spaceship = new SpaceshipInput();

        spaceshipsInput.Add(spaceship);

        wpSpaceShip.Children.Add(insertNewEntity(
            spaceship,
            "Spaceships",
            "#A7CECB",
            textBoxSpaceShip_TextChanged,
            btnSpaceshipDelete_Click,
            comboBoxSpaceship_SelectionChanged
        ));
    }
    private void btnSpaceshipUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (spaceshipsInput is null) return; // prevent button click while waiting for the async load function
        PropertyInfo[] properties_ss_input = typeof(SpaceshipInput).GetProperties().Skip(1).ToArray();
        PropertyInfo[] properties_ss = typeof(SpaceshipObject).GetProperties().Skip(1).ToArray();
        var collection = database.GetCollection<SpaceshipObject>("Spaceships");
        string errorMsg = "";
        spaceships.Clear();
        foreach (var spaceship in spaceshipsInput)
        {
            SpaceshipObject newSpaceShip = new SpaceshipObject(spaceship.id);
            verifyUserInput(
                ref errorMsg,
                ref newSpaceShip,
                properties_ss,
                properties_ss_input,
                spaceship
            );
            spaceships.Add(newSpaceShip);
        }

        string[] new_class_list = spaceships.Select(obj => obj.Class).Distinct().ToArray();
        var collection_officers = database.GetCollection<OfficerObject>("Officers");
        collection_officers.DeleteMany(Builders<OfficerObject>.Filter.Nin(x => x.ShipSpecialty, new_class_list));

        uploadDB(collection, errorMsg, spaceships);
    }
    private async void btnSpaceshipLoad_Click(object sender, RoutedEventArgs e)
    {
        dbLoading(SpaceshipErrLabel);
        const string _collection_name = "Spaceships";
        List<SpaceshipObject> collection = await database.GetCollection<SpaceshipObject>(_collection_name).Find(_ => true).As<SpaceshipObject>().ToListAsync();

        wpSpaceShip.Children.Clear();
        spaceships = new List<SpaceshipObject>();
        spaceshipsInput = new List<SpaceshipInput>();
        foreach (SpaceshipObject _obj in collection)
        {
            SpaceshipInput _obj_input = new SpaceshipInput(_obj);
            spaceshipsInput.Add(_obj_input);
            wpSpaceShip.Children.Add(insertNewEntity(
                _obj_input,
                _collection_name,
                "#A7CECB",
                textBoxSpaceShip_TextChanged,
                btnSpaceshipDelete_Click,
                comboBoxSpaceship_SelectionChanged
            ));
        }

        dbLoadComplete(SpaceshipErrLabel, _collection_name);
    }
    private void btnSpaceshipDelete_Click(object sender, RoutedEventArgs e)
    {
        removeEntity(sender, e, spaceshipsInput, wpSpaceShip);
    }
    private void textBoxSpaceShip_TextChanged(object sender, RoutedEventArgs e)
    {
        textBox_TextChanged(sender, e, spaceshipsInput, SpaceshipErrLabel);
    }
    private void comboBoxSpaceship_SelectionChanged(object sender, RoutedEventArgs e)
    {
        comboBox_SelectionChanged(sender, e, ref spaceshipsInput);
    }

    /* ----------------- OFFICER TAB ----------------- */
    private void btnOfficerAdd_Click(object sender, RoutedEventArgs e)
    {
        if (officersInput is null) return; // prevent button click while waiting for the async load function
        OfficerInput officer = new OfficerInput();

        officersInput.Add(officer);

        wpOfficer.Children.Add(insertNewEntity(
            officer,
            "Officers",
            "#0BB641",
            textBoxOfficer_TextChanged,
            btnOfficerDelete_Click,
            comboBoxOfficer_SelectionChanged
        ));
    }
    private void btnOfficerUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (officersInput is null) return; // prevent button click while waiting for the async load function
        PropertyInfo[] properties_of_input = typeof(OfficerInput).GetProperties().Skip(1).ToArray();
        PropertyInfo[] properties_of = typeof(OfficerObject).GetProperties().Skip(1).ToArray();
        var collection = database.GetCollection<OfficerObject>("Officers");
        string errorMsg = "";
        officers.Clear();
        foreach (var officer in officersInput)
        {
            OfficerObject newOfficer = new OfficerObject(officer.id);
            verifyUserInput(
                ref errorMsg,
                ref newOfficer,
                properties_of,
                properties_of_input,
                officer
            );
            officers.Add(newOfficer);
        }

        uploadDB(collection, errorMsg, officers);
    }
    private async void btnOfficerLoad_Click(object sender, RoutedEventArgs e)
    {
        dbLoading(OfficerErrLabel);
        const string _collection_name = "Officers";
        List<OfficerObject> collection = await database.GetCollection<OfficerObject>(_collection_name).Find(_ => true).As<OfficerObject>().ToListAsync();
        
        shipSpecialties = await database.GetCollection<SpaceshipObject>("Spaceships").AsQueryable().Select(d => d.Class).ToListAsync();
        planetSystems = await database.GetCollection<PlanetObject>("Planetary System").AsQueryable().Select(d => d.Name).ToListAsync();

        wpOfficer.Children.Clear();
        officers = new List<OfficerObject>();
        officersInput = new List<OfficerInput>();
        foreach (OfficerObject _obj in collection)
        {
            OfficerInput _obj_input = new OfficerInput(_obj);
            officersInput.Add(_obj_input);
            wpOfficer.Children.Add(insertNewEntity(
                _obj_input,
                _collection_name,
                "#0BB641",
                textBoxOfficer_TextChanged,
                btnOfficerDelete_Click,
                comboBoxOfficer_SelectionChanged
            ));
        }

        dbLoadComplete(OfficerErrLabel, _collection_name);
    }
    private void btnOfficerDelete_Click(object sender, RoutedEventArgs e)
    {
        removeEntity(sender, e, officersInput, wpOfficer);
    }
    private void textBoxOfficer_TextChanged(object sender, RoutedEventArgs e)
    {
        textBox_TextChanged(sender, e, officersInput, OfficerErrLabel);
    }
    private void comboBoxOfficer_SelectionChanged(object sender, RoutedEventArgs e)
    {
        comboBox_SelectionChanged(sender, e, ref officersInput);
    }

    /* ----------------- PLANET TAB ----------------- */
    private void btnPlanetAdd_Click(object sender, RoutedEventArgs e)
    {
        if (planetsInput is null) return;
        PlanetInput planet = new PlanetInput();

        planetsInput.Add(planet);

        wpPlanet.Children.Add(insertNewEntity(
            planet,
            "Planet",
            "#A8A9C9",
            textBoxPlanet_TextChanged,
            btnPlanetDelete_Click,
            comboBoxPlanet_SelectionChanged
        ));
    }
    private void btnPlanetUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (planetsInput is null) return;
        PropertyInfo[] properties_pl_input = typeof(PlanetInput).GetProperties().Skip(1).ToArray();
        PropertyInfo[] properties_pl = typeof(PlanetObject).GetProperties().Skip(1).ToArray();
        var collection = database.GetCollection<PlanetObject>("Planetary System");
        string errorMsg = "";
        planets.Clear();
        foreach (var planet in planetsInput)
        {
            PlanetObject newPlanet = new PlanetObject(planet.id);
            verifyUserInput(
                ref errorMsg,
                ref newPlanet,
                properties_pl,
                properties_pl_input,
                planet
            );
            planets.Add(newPlanet);
        }

        string[] new_planet_list = planets.Select(obj => obj.Name).Distinct().ToArray();
        
        var collection_officers = database.GetCollection<OfficerObject>("Officers");
        collection_officers.DeleteMany(Builders<OfficerObject>.Filter.Nin(x => x.HomePlanetSystem, new_planet_list));

        var collection_missions = database.GetCollection<MissionObject>("Missions");
        collection_missions.DeleteMany(Builders<MissionObject>.Filter.Nin(x => x.Location, new_planet_list));

        uploadDB(collection, errorMsg, planets);
    }
    private async void btnPlanetLoad_Click(object sender, RoutedEventArgs e)
    {
        dbLoading(PlanetErrLabel);
        const string _collection_name = "Planetary System";
        List<PlanetObject> collection = await database.GetCollection<PlanetObject>(_collection_name).Find(_ => true).As<PlanetObject>().ToListAsync();

        wpPlanet.Children.Clear();
        planetsInput = new List<PlanetInput>();
        planets = new List<PlanetObject>();
        foreach (PlanetObject _obj in collection)
        {
            PlanetInput _obj_input = new PlanetInput(_obj);
            planetsInput.Add(_obj_input);
            wpPlanet.Children.Add(insertNewEntity(
                _obj_input,
                _collection_name,
                "#A8A9C9",
                textBoxPlanet_TextChanged,
                btnPlanetDelete_Click,
                comboBoxPlanet_SelectionChanged
            ));
        }

        dbLoadComplete(PlanetErrLabel, _collection_name);
    }
    private void btnPlanetDelete_Click(object sender, RoutedEventArgs e)
    {
        removeEntity(sender, e, planetsInput, wpPlanet);
    }
    private void textBoxPlanet_TextChanged(object sender, RoutedEventArgs e)
    {
        textBox_TextChanged(sender, e, planetsInput, PlanetErrLabel);
    }
    private void comboBoxPlanet_SelectionChanged(object sender, RoutedEventArgs e)
    {
        comboBox_SelectionChanged(sender, e, ref planetsInput);
    }

    /* ----------------- MISSION FUNCTIONS ----------------- */
    private void btnMissionAdd_Click(object sender, RoutedEventArgs e)
    {
        if (missions is null) return;
        MissionObject mission = new MissionObject();

        missions.Add(mission);

        wpMission.Children.Add(insertNewEntity(
            mission,
            "Missions",
            "#FF92C2",
            textBoxMission_TextChanged,
            btnMissionDelete_Click,
            comboBoxMission_SelectionChanged
        ));
    }
    private void btnMissionUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (missions is null) return;
        PropertyInfo[] properties_ms = typeof(MissionObject).GetProperties().Skip(1).ToArray();
        var collection = database.GetCollection<MissionObject>("Missions");
        string errorMsg = "";
        foreach (var mission in missions)
        {
            // Mission does not need checkers since all properties are strings
            foreach(PropertyInfo _prop in properties_ms)
            {
                if (string.IsNullOrEmpty((string)_prop.GetValue(mission)))
                {
                    errorMsg += $"•{_prop.Name} cannot be empty\n";
                }
            }
        }

        uploadDB(collection, errorMsg, missions);
    }
    private async void btnMissionLoad_Click(object sender, RoutedEventArgs e)
    {
        dbLoading(MissionErrLabel);
        const string _collection_name = "Missions";
        List<MissionObject> collection = await database.GetCollection<MissionObject>(_collection_name).Find(_ => true).As<MissionObject>().ToListAsync();

        planetSystems = await database.GetCollection<PlanetObject>("Planetary System").AsQueryable().Select(d => d.Name).ToListAsync();

        wpMission.Children.Clear();
        missions = collection;
        foreach (MissionObject _obj in missions)
        {
            wpMission.Children.Add(insertNewEntity(
                _obj,
                _collection_name,
                "#FF92C2",
                textBoxMission_TextChanged,
                btnMissionDelete_Click,
                comboBoxMission_SelectionChanged
            ));
        }

        dbLoadComplete(MissionErrLabel, _collection_name);
    }
    private void btnMissionDelete_Click(object sender, RoutedEventArgs e)
    {
        removeEntity(sender, e, missions, wpMission);
    }
    private void textBoxMission_TextChanged(object sender, RoutedEventArgs e)
    {
        textBox_TextChanged(sender, e, missions, MissionErrLabel);
    }
    private void comboBoxMission_SelectionChanged(object sender, RoutedEventArgs e)
    {
        comboBox_SelectionChanged(sender, e, ref missions);
    }

    /* ----------------- UTIL FUNCTIONS ----------------- */
    private void dbLoading(Label _label)
    {
        _label.Foreground = (Brush)bc.ConvertFrom("Black");
        _label.Content = "Loading Collections...";
    }

    private void dbLoadComplete(Label _label, string _collection)
    {
        _label.Foreground = (Brush)bc.ConvertFrom("Green");
        _label.Content = $"{_collection} Reload Complete";
    }

    private void verifyUserInput<T, P>(ref string _errMsg, ref T _data_obj, PropertyInfo[] _props_obj, PropertyInfo[] _props_input, P _data_input)
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
                else
                {
                    _props_obj[i].SetValue(_data_obj, spInputVal);
                }
            }
        }
    }

    private void uploadDB<T>(IMongoCollection<T> collection, string errorMsg, List<T> _obj_arr)
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

    private void textBox_TextChanged<T>(object sender, RoutedEventArgs e, List<T> _data_list, Label _err_label)
    {
        if (!this.IsLoaded) return;
        string txtbxName = ((TextBox)sender).Name;
        string txtbxTxt = ((TextBox)sender).Text;
        string keyName = txtbxName.Split("NAME")[1];
        string objID = txtbxName.Split("NAME")[0].Split("ID")[1];
        PropertyInfo property = typeof(T).GetProperty(keyName);
        int index = _data_list.FindIndex(obj => typeof(T).GetProperty("id").GetValue(obj).ToString() == objID);

        if (string.IsNullOrEmpty(txtbxTxt))
        {
            _err_label.Foreground = (Brush)bc.ConvertFrom("Red");
            _err_label.Content = $"{keyName} cannot be empty";
        }
        else
        {
            _err_label.Content = "";
            property.SetValue(_data_list[index], txtbxTxt);
        }
    }

    private void comboBox_SelectionChanged<T>(object sender, RoutedEventArgs e, ref List<T> _obj_input_arr)
    {
        if (!this.IsLoaded) return;
        string[] txtbxName = ((ComboBox)sender).Name.Split("NAME");
        string txtbxTxt = ((ComboBox)sender).SelectedItem.ToString();
        int index = _obj_input_arr.FindIndex(obj => typeof(T).GetProperty("id").GetValue(obj).ToString() == txtbxName[0].Split("ID")[1]);

        typeof(T).GetProperty(txtbxName[1]).SetValue(_obj_input_arr[index], txtbxTxt);
    }

    private void removeEntity<T>(object sender, RoutedEventArgs e, List<T> _data_input, WrapPanel _wp)
    {
        string delBtnName = ((Button)sender).Name;
        string delEntityID = delBtnName.Split("ID")[1].Split("DELETE")[0];

        int delIdx = _data_input.FindIndex(item => typeof(T).GetProperty("id").GetValue(item).ToString() == delEntityID);
        _data_input.RemoveAt(delIdx);
        _wp.Children.RemoveAt(delIdx);
    }

    private StackPanel insertNewEntity<T>(T _obj, string _obj_name, string bgColor, TextChangedEventHandler _f_txt_onchange, RoutedEventHandler _f_del_entity, SelectionChangedEventHandler _f_cbox_onselect)
    {
        string _obj_id = typeof(T).GetProperty("id").GetValue(_obj).ToString();
        PropertyInfo[] _obj_properties = typeof(T).GetProperties().Skip(1).ToArray();
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
            if (propertyName == "Class" && _obj_name == "Spaceships")
            {
                ComboBox entityTextBox = new ComboBox();
                entityTextBox.Width = 120;
                entityTextBox.Name = $"ID{_obj_id}NAME{propertyName}";

                foreach (ShipSpecialty sp in Enum.GetValues(typeof(ShipSpecialty)).Cast<ShipSpecialty>())
                {
                    entityTextBox.Items.Add(sp.ToString());
                }
                entityTextBox.SelectedItem = propertyValue;
                entityTextBox.SelectionChanged += _f_cbox_onselect;

                spEntity.Children.Add(entityTextBox);
            }
            else if (propertyName == "ShipSpecialty" && _obj_name == "Officers")
            {
                ComboBox entityTextBox = new ComboBox();
                entityTextBox.Width = 120;
                entityTextBox.Name = $"ID{_obj_id}NAME{propertyName}";

                foreach (var avail_sp in shipSpecialties)
                {
                    entityTextBox.Items.Add(avail_sp.ToString());
                }
                entityTextBox.SelectionChanged += _f_cbox_onselect;
                entityTextBox.SelectedItem = propertyValue;

                spEntity.Children.Add(entityTextBox);
            }
            else if (
                (propertyName == "HomePlanetSystem" && _obj_name == "Officers") ||
                (propertyName == "Location" && _obj_name == "Missions")
            )
            {
                ComboBox entityTextBox = new ComboBox();
                entityTextBox.Width = 120;
                entityTextBox.Name = $"ID{_obj_id}NAME{propertyName}";

                foreach (var avail_sp in planetSystems)
                {
                    entityTextBox.Items.Add(avail_sp.ToString());
                }
                entityTextBox.SelectionChanged += _f_cbox_onselect;
                entityTextBox.SelectedItem = propertyValue;

                spEntity.Children.Add(entityTextBox);
            }
            else
            {
                TextBox entityTextBox = new TextBox();
                entityTextBox.Width = 120;
                entityTextBox.Name = $"ID{_obj_id}NAME{propertyName}";
                entityTextBox.TextWrapping = TextWrapping.Wrap;
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