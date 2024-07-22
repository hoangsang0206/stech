using STech.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace STech.Services.Services
{
    public class AddressService
    {
        private readonly string ROOT_PATH;
        private readonly string CITIES_FILE_PATH = Path.Combine("Json", "Address", "cities.json");
        private readonly string DISTRICTS_FILE_PATH = Path.Combine("Json", "Address", "districts.json");
        private readonly string WARDS_FILE_PATH = Path.Combine("Json", "Address", "wards.json");

        public List<AddressVM.City> Cities { get; private set; } = new List<AddressVM.City>();
        public List<AddressVM.District> Districts { get; private set; } = new List<AddressVM.District>();
        public List<AddressVM.Ward> Wards { get; private set; } = new List<AddressVM.Ward>();

        public AddressService(string rootPath)
        {
            ROOT_PATH = rootPath;
            LoadCities().Wait();
            LoadDistricts().Wait();
            LoadWards().Wait();

            Cities.ForEach(city =>
            {
                city.districts = Districts.Where(d => d.parent_code == city.code).OrderBy(d => d.slug).ToList();
            });

            Districts.ForEach(district =>
            {
                district.wards = Wards.Where(w => w.parent_code == district.code).OrderBy(w => w.slug).ToList();
            });
        }

        private async Task<List<T>?> ReadJson<T>(string relativePath)
        {
            string filePath = Path.Combine(ROOT_PATH, relativePath);
            if (File.Exists(filePath))
            {
                try
                {
                    string jsonContent = await File.ReadAllTextAsync(filePath);
                    return JsonSerializer.Deserialize<List<T>>(jsonContent);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        private async Task LoadCities()
        {
            Cities = await ReadJson<AddressVM.City>(CITIES_FILE_PATH) ?? new List<AddressVM.City>();
            Cities = Cities.OrderBy(c => c.slug).ToList();
        }

        private async Task LoadDistricts()
        {
            Districts = await ReadJson<AddressVM.District>(DISTRICTS_FILE_PATH) ?? new List<AddressVM.District>();
        }

        private async Task LoadWards()
        {
            Wards = await ReadJson<AddressVM.Ward>(WARDS_FILE_PATH) ?? new List<AddressVM.Ward>();
        }
    }
}
