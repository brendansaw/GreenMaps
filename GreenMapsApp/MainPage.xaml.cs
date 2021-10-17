﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GreenMapsApp.Model;
using System.Net;
using System.Linq;

namespace GreenMapsApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {

            InitializeComponent();

            RestService restService = new RestService();
            MapHelperFunctions mapHelper = new MapHelperFunctions();

            Map map = new Map
            {
                IsShowingUser = true,
                
            };

            mapHelper.FindMe(map);

            async void OnMapClicked(object sender, MapClickedEventArgs e)
            {
                bool answer = await DisplayAlert("Would you like to add a pin", "", "Yes", "No");
                if (answer)
                {
                    string label = await DisplayPromptAsync("Add Title","", initialValue: "", maxLength: 16, keyboard: Keyboard.Default);
                    string description = await DisplayPromptAsync("Add Description", "", initialValue: "", maxLength: 64, keyboard: Keyboard.Default);
                    Pin pin = new Pin
                    {
                        Label = label,
                        Position = new Position(e.Position.Latitude, e.Position.Longitude),
                        Address = description
                    };

                    MapLocationDatum outputJson = new MapLocationDatum();
                    foreach(IPAddress address in Dns.GetHostAddresses(Dns.GetHostName()))
                    {
                        outputJson.ipAddress = address.ToString();
                        break;
                    }
                    outputJson.latitude = e.Position.Latitude;
                    outputJson.longitude = e.Position.Longitude;
                    outputJson.title = label;
                    outputJson.resolved = false;
                    outputJson.message = description;
                    outputJson.dateCreated = DateTime.Now;

                    string json = JsonConvert.SerializeObject(outputJson);
                    restService.Post(json);
                    map.Pins.Add(pin);
                }
            }

            map.MapClicked += OnMapClicked;

            mapHelper.PopulateMap(map);

            StackLayout stackLayout = new StackLayout
            {
                Margin = new Thickness(0),
                Children =
                {
                    new Label { Text = "Primary colors",TextColor = Color.FromHex("#77d065"), FontSize = 20 },
                    map,
                }
            };

            Content = stackLayout;
        }
    }
}

