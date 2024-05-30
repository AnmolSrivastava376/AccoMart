using Data.Models.Address;
using System.Collections.Generic;

public class AddressModelComparer : IComparer<AddressModel>
{
    public int Compare(AddressModel x, AddressModel y)
    {
        if (x == null && y == null)
        {
            return 0;
        }
        else if (x == null)
        {
            return -1;
        }
        else if (y == null)
        {
            return 1;
        }
        else
        {
            // Compare properties of AddressModel objects
            int comparison = x.AddressId.CompareTo(y.AddressId);
            if (comparison != 0)
            {
                return comparison;
            }
            comparison = x.Street.CompareTo(y.Street);
            if (comparison != 0)
            {
                return comparison;
            }
            comparison = x.City.CompareTo(y.City);
            if (comparison != 0)
            {
                return comparison;
            }
            comparison = x.PhoneNumber.CompareTo(y.PhoneNumber);
            if (comparison != 0)
            {
                return comparison;
            }
            comparison = x.State.CompareTo(y.State);
            if (comparison != 0)
            {
                return comparison;
            }
            return x.ZipCode.CompareTo(y.ZipCode);
        }
    }
}
