using Core.Attributes;
using Core.Configurations;
using Core.Dtos;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using ScreenCapture.WebApp.Services.Interfaces;
using System.Reflection;

namespace ScreenCapture.WebApp.Services.Implementers
{
    public class LocalStorageDtoFactory : IDtoFactory
    {
        private readonly ProtectedLocalStorage _storage;
        private readonly Dictionary<string, SettingsGroupConfiguration> _defaultConfiguratios;

        public LocalStorageDtoFactory(ProtectedLocalStorage storage, IOptions<Dictionary<string, SettingsGroupConfiguration>> options)
        {
            _storage = storage;
            _defaultConfiguratios = options.Value;
        }

        public Task<RecordingOptions> CreateRecordingOptionsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ScreenshotOptions> CreateScreenshotOptionsAsync()
        {
            Type type = typeof(ScreenshotOptions);
            var dto = new ScreenshotOptions();
            dto.ImageFormat = await GetSettingValue(type, nameof(dto.ImageFormat));
            return dto;
        }

        private async Task<string> GetSettingValue(Type type, string propertyName)
        {
            var attribute = type.GetProperty(propertyName)!.GetCustomAttribute<SettingKeyAttribute>(false);
            if (attribute == null || string.IsNullOrWhiteSpace(attribute.KeyName))
            {
                throw new InvalidOperationException($"The property {propertyName} of the setting related class {type.Name} has not been flagged with a key attribute.");
            }

            var propertyKey = attribute.KeyName;
            var storageResult = await _storage.GetAsync<string>(propertyKey);
            if (storageResult.Success && storageResult.Value != null)
            {
                return storageResult.Value;
            }

            // fallback to default value if no stored key has been found
            attribute = type.GetCustomAttribute<SettingKeyAttribute>(false);
            if (attribute == null || string.IsNullOrWhiteSpace(attribute.KeyName))
            {
                throw new InvalidOperationException($"The  class {type.Name} has not been flagged with a key attribute.");
            }

            return _defaultConfiguratios[attribute.KeyName].Settings[propertyKey].DefaultValue;
        }
    }
}
