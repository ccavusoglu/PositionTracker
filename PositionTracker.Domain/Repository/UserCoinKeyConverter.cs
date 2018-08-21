using System;
using System.ComponentModel;
using System.Globalization;
using PositionTracker.Domain.Entity;

namespace PositionTracker.Domain.Repository
{
    public class UserCoinKeyConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(UserCoinKey) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var parts = value?.ToString().Split('_');

            return UserCoinKey.Get(parts?[0], parts?[1]);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            var obj = value as UserCoinKey;

            return UserCoinKey.GetUniqueId(obj?.Coin, obj?.Exchange);
        }
    }
}