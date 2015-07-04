/**
 *  ONTRACK DATABASE
 *  
 *  DATATYPE ROUTINES
 * 
 * Version: 1.0
 * Created: 2015-04-14
 * Last Change
 * 
 * Change Log
 * 
 * (C) by Boris Schneider, 2015
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;

namespace OnTrack.Core
{
    /// <summary>
    /// static class Datatype
    /// </summary>
    public static class DataType
    {
        public const Char ConstDelimiter = '|';
        public const String ConstNullTimestampString = "1900-01-01T00:00:00";
        /// <summary>
        /// returns the best fit System.Type for a OnTrack Datatype
        /// </summary>
        /// <param name="datatype"></param>
        /// <returns></returns>
        public static System.Type GetTypeFor(otDataType datatype)
        {
            switch (datatype)
            {
                case otDataType.Void:
                    return typeof(void);
                case otDataType.Date:
                    return typeof(DateTime);
                case otDataType.Bool:
                    return typeof(bool);
                case otDataType.List:
                    return typeof(List<string>);
                case otDataType.Long:
                    return typeof(long);
                case otDataType.Memo:
                    return typeof(string);
                case otDataType.Text:
                    return typeof(string);
                case otDataType.Time:
                    return typeof(TimeSpan);
                case otDataType.Numeric:
                    return typeof(double);
                case otDataType.Timestamp:
                    return typeof(DateTime);
                default:
                    throw new NotImplementedException("mapping for '" + datatype.ToString() + "' is not implemented");
            }
        }
        /// <summary>
        /// returns a default value for the OnTrack Datatypes
        /// </summary>
        /// <param name="datatype"></param>
        /// <returns></returns>
        public static object GetDefaultValue(otDataType datatype)
        {

            switch (datatype)
            {
                case otDataType.Bool:
                    return false;
                case otDataType.Date:
                    return  DateTime.Parse(ConstNullTimestampString ).Date ;
                case otDataType.List:
                    /// To do implement inner Type
                    /// or accept Object()
                    List<string> aValue = new List<string>();
                    return aValue.ToArray();
                case otDataType.Long:
                    return (long) 0;
                case otDataType.Memo:
                    return string.Empty;
                case otDataType.Numeric:
                    return (double) 0;
                case otDataType.Text:
                    return string.Empty;
                case otDataType.Time:
                    return new TimeSpan();
                case otDataType.Timestamp:
                    return DateTime.Parse(ConstNullTimestampString);
                default:
                    throw new NotImplementedException("default value for '" + datatype.ToString() + "' is not implemented");
                    return null;
            }

        }
        /// <summary>
        /// returns true if the value is convertible to the datatype
        /// </summary>
        /// <param name="value"></param>
        /// <param name="outvalue"></param>
        /// <param name="datatype"></param>
        /// <returns></returns>
        public static bool Is(object value, otDataType datatype)
        {
            switch (datatype)
            {
                case otDataType.Bool:
                    return IsBool(value);
                case otDataType.Date:
                    return IsDate(value);
                case otDataType.List:
                    return IsList(value);
                case otDataType.Long:
                    return IsLong(value);
                case otDataType.Memo:
                    return IsMemo(value);
                case otDataType.Numeric:
                    return IsNumeric(value);
                case otDataType.Text:
                    return IsText(value);
                case otDataType.Time:
                    return IsTime(value);
                case otDataType.Timestamp:
                    return IsTimeStamp(value);
                default:
                    throw new NotImplementedException("convert value for '" + datatype.ToString() + "' is not implemented");
                    return false;
            }
        }
       
        /// <summary>
        /// converts a value to an representing value of the outvalue
        /// </summary>
        /// <param name="value"></param>
        /// <param name="outvalue"></param>
        /// <param name="datatype"></param>
        /// <returns></returns>
        public static object To(ref object value, otDataType datatype)
        {
            switch (datatype)
            {
                case otDataType.Bool:
                    return ToBool (value);
                case otDataType.Date:
                    return ToDate(value);
                case otDataType.List:
                   return ToList(value);
                case otDataType.Long:
                    return ToLong(value);
                case otDataType.Memo:
                    return ToMemo(value);
                case otDataType.Numeric:
                    return ToNumeric(value);
                case otDataType.Text:
                    return ToText(value);
                case otDataType.Time:
                    return ToTime(value);
                case otDataType.Timestamp:
                    return ToTimeStamp(value);
                default:
                    throw new NotImplementedException("convert value for '" + datatype.ToString() + "' is not implemented");
                    return false;
            }
        }
         /// <summary>
         /// returns true if the value is of otDataType.bool
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static bool IsBool(object value)
         {
             // if it is a bool anyway
             if (value != null && (value.GetType() == typeof(bool) || value.GetType() == typeof(Boolean))) return true;

             // try to convert to number if that works -> convertible
             if (value != null)
             {
                 bool bvalue;
                 if (bool.TryParse(value.ToString(), out bvalue)) return true;
                 float fvalue;
                 if (float.TryParse(value.ToString(), out fvalue)) return true;
             }

             return false; // not convertible
         }
         /// <summary>
         /// convert a value to otDataType.Bool and return the value
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static bool ToBool(object value)
         {
             // if it is a bool anyway
             if (value != null && (value.GetType() == typeof(bool) || value.GetType() == typeof(Boolean))) return (bool)value;

             // try to convert to number if that works -> convertible
             if (value != null)
             {
                 // convert True, False to bool
                 bool bvalue;
                 if (bool.TryParse(value.ToString(), out bvalue)) return bvalue;
                 // every numeric value except 0 is regarded as true
                 float fvalue;
                 if (float.TryParse(value.ToString(), out fvalue))
                 {
                     if (fvalue == 0) return false;
                     else return true;
                 }
             }

             if (value == null) value = "(null)";
             // throw exception
             throw new Rulez.RulezException(Rulez.RulezException.Types.ValueNotConvertible, arguments: new object[] { value, "bool" });
         }
         /// <summary>
         /// returns true if the value is of otDataType.Date
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static bool IsDate(object value)
         {
             // if it is a type anyway
             if (value != null && value.GetType() == typeof(DateTime)) return true;

             // try to convert to number if that works -> convertible
             if (value != null)
             {
                 DateTime dtvalue;
                 // if this is time (no date -> converted to today) then check with second expression
                 // 21.05.2015 10:00 -> is Timestamp not a date !
                 if ((DateTime.TryParse(value.ToString(), out dtvalue)) && (dtvalue.TimeOfDay == dtvalue.Date.TimeOfDay ))  return true;
             }

             return false; // not convertible
         }
         /// <summary>
         /// convert a value to otDataType.Date and return the value
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static DateTime ToDate(object value)
         {
             // if it is a type anyway
             if (value != null && value.GetType() == typeof(DateTime)) return (DateTime)value;

             // try to convert to datetime
             if (value != null)
             {
                 // convert just the date component of the value
                 DateTime dtvalue;
                 if (DateTime.TryParse(value.ToString(), out dtvalue)) 
                 {
                     return dtvalue.Date;
                 }
             }

             if (value == null) value = "(null)";
             // throw exception
             throw new Rulez.RulezException(Rulez.RulezException.Types.ValueNotConvertible, arguments: new object[] { value, "date" });
         }
         /// <summary>
         /// returns true if the value is of otDataType.Time
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static bool IsTime(object value)
         {
             // if it is a type anyway
             if (value != null && value.GetType() == typeof(DateTime) || value.GetType() == typeof(TimeSpan)) return true;

             // try to convert to number if that works -> convertible
             if (value != null)
             {
                 TimeSpan tsvalue;
                 if (TimeSpan.TryParse(value.ToString(), out tsvalue)) return true;
                 DateTime dtvalue;
                 // if this is time (no date -> converted to today) then check with second expression
                 // 21.05.2015 10:00 -> is Timestamp not a timespan !
                 if ((DateTime.TryParse(value.ToString(), out dtvalue)) && (dtvalue.TimeOfDay != dtvalue.Date.TimeOfDay)) return true;
             }

             return false; // not convertible
         }
         /// <summary>
         /// convert a value to otDataType.Time and return the value
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static TimeSpan ToTime(object value)
         {
             // if it is anyway the right type
             if (value != null && value.GetType() == typeof(TimeSpan)) return ((TimeSpan)value);
             if (value.GetType() == typeof(DateTime)) return ((DateTime)value).TimeOfDay ;

             // try to convert to datetime
             if (value != null)
             {
                 // convert just the timespan
                 TimeSpan tsvalue;
                 if (TimeSpan.TryParse(value.ToString(), out tsvalue)) return tsvalue;
                
                 // if this is time (no date -> converted to today) then check with second expression
                 // 21.05.2015 10:00 -> is Timestamp not a timespan !
                 DateTime dtvalue;
                 if ((DateTime.TryParse(value.ToString(), out dtvalue)) && (dtvalue.TimeOfDay != dtvalue.Date.TimeOfDay)) return dtvalue.TimeOfDay ;
             }

             if (value == null) value = "(null)";
             // throw exception
             throw new Rulez.RulezException(Rulez.RulezException.Types.ValueNotConvertible, arguments: new object[] { value, "time" });
         }
         /// <summary>
         /// returns true if the value is of otDataType.TimeStamp
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static bool IsTimeStamp(object value)
         {
             // if it is a type anyway
             if (value != null && value.GetType() == typeof(DateTime)) return true;

             // try to convert to number if that works -> convertible
             if (value != null)
             {
                 DateTime dtvalue;
                 // if this is time (no date -> converted to today) then check with second expression
                 // 21.05.2015 10:00 -> is Timestamp not a timespan !
                 if ((DateTime.TryParse(value.ToString(), out dtvalue))) return true;
             }

             return false; // not convertible
         }
         /// <summary>
         /// convert a value to otDataType.Timestamp and return the value
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static DateTime ToTimeStamp(object value)
         {
             // if it is anyway the right type
             if (value != null && value.GetType() == typeof(DateTime)) return ((DateTime)value);

             // try to convert to datetime
             if (value != null)
             {

                 // if this is time (no date -> converted to today) then check with second expression
                 // 21.05.2015 10:00 -> is Timestamp not a timespan !
                 DateTime dtvalue;
                 if ((DateTime.TryParse(value.ToString(), out dtvalue))) return dtvalue;
             }

             if (value == null) value = "(null)";
             // throw exception
             throw new Rulez.RulezException(Rulez.RulezException.Types.ValueNotConvertible, arguments: new object[] { value, "timestamp" });
         }
         /// <summary>
         /// returns true if the value is of otDataType.Numeric
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static bool IsNumeric(object value)
         {
             // if it is a type anyway
             if (value != null && (value.GetType() == typeof(Double) || value.GetType() == typeof(float) || value.GetType()== typeof(Single) 
                 || value.GetType() == typeof(long) || value.GetType() == typeof(int))) return true;

             // try to convert to number if that works -> convertible
             if (value != null)
             {
                 Double dvalue;
                 if (Double.TryParse (value.ToString(), out dvalue))  return true;
             }

             return false; // not convertible
         }
         /// <summary>
         /// convert a value to otDataType.Double and return the value
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static Double ToNumeric(object value)
         {
             // if it is anyway the right type
             if (value != null && value.GetType() == typeof(Double)) return ((Double)value);
             if (value != null && value.GetType() == typeof(Single)) return ((Double)value);
             if (value != null && value.GetType() == typeof(Decimal)) return ((Double)value);

             // try to convert to datetime
             if (value != null)
             {
                 Double dvalue;
                 if (Double.TryParse(value.ToString(), out dvalue)) return dvalue;
             }

             if (value == null) value = "(null)";
             // throw exception
             throw new Rulez.RulezException(Rulez.RulezException.Types.ValueNotConvertible, arguments: new object[] { value, "numeric" });
         }
         /// <summary>
         /// returns true if the value is of otDataType.Long
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static bool IsLong(object value)
         {
             // if it is a type anyway
             if (value != null && (value.GetType() == typeof(long) || value.GetType() == typeof(int))) return true;

             // try to convert to number if that works -> convertible
             if (value != null)
             {
                 long lvalue;
                 if (long.TryParse(value.ToString(), out lvalue)) return true;
             }

             return false; // not convertible
         }
         /// <summary>
         /// convert a value to otDataType.Long and return the value
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static Double ToLong(object value)
         {
             // if it is anyway the right type
             if (value != null && value.GetType() == typeof(Double)) return ((Double)value);

             // try to convert to datetime
             if (value != null)
             {
                 // convert to long
                 long lvalue;
                 if (long.TryParse(value.ToString(), out lvalue)) return lvalue;
                 // loose
                 decimal dvalue;
                 if (decimal.TryParse(value.ToString(), out dvalue))
                 {
                     return (long) Math.Round (dvalue);
                 }
             }

             if (value == null) value = "(null)";
             // throw exception
             throw new Rulez.RulezException(Rulez.RulezException.Types.ValueNotConvertible, arguments: new object[] { value, "long" });
         }
         /// <summary>
         /// returns true if the value is of otDataType.Text
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static bool IsText(object value)
         {
             // if it is a type anyway
             if (value != null && value.GetType() == typeof(String)) return true;

             // toString
             if (value != null) return true;
            

             return false; // not convertible
         }
         /// <summary>
         /// convert a value to otDataType.Text and return the value
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static String ToText(object value)
         {
             // if it is anyway the right type
             if (value != null && value.GetType() == typeof(String)) return ((String)value);

             // try to convert 
             if (value != null)
             {
                 // convert to long
                 return value.ToString();
             }

             if (value == null) value = "(null)";
             // throw exception
             throw new Rulez.RulezException(Rulez.RulezException.Types.ValueNotConvertible, arguments: new object[] { value, "text" });
         }
         /// <summary>
         /// returns true if the value is of otDataType.Text
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static bool IsMemo(object value)
         {
             // if it is a type anyway
             if (value != null && value.GetType() == typeof(String)) return true;

             // toString
             if (value != null) return true;


             return false; // not convertible
         }
         /// <summary>
         /// convert a value to otDataType.Text and return the value
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static String ToMemo(object value)
         {
             // if it is anyway the right type
             if (value != null && value is String) return ((String)value);

             // try to convert 
             if (value != null)
             {
                 // convert to long
                 return value.ToString();
             }

             if (value == null) value = "(null)";
             // throw exception
             throw new Rulez.RulezException(Rulez.RulezException.Types.ValueNotConvertible, arguments: new object[] { value, "text" });
         }
         /// <summary>
         /// returns true if the value is of otDataType.Text
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static bool IsList(object value)
         {
             // if it is a type anyway
             if (value != null && (value.GetType().IsArray || value.GetType().IsAssignableFrom (typeof(List<>)))) return true;
             

             // toString
             if (value != null && value is String && ((String)value).Contains('|')) return true;

             return false; // not convertible
         }
         /// <summary>
         /// convert a value to otDataType.Text and return the value
         /// </summary>
         /// <param name="value"></param>
         /// <returns></returns>
         public static List<String> ToList(object value)
         {
             // try to convert 
             if (value != null)
             {
                 if (value.GetType().IsAssignableFrom(typeof(List<>))) return ((IEnumerable)value).Cast<object>().Select(x => x.ToString()).ToList(); ;
                 if (value.GetType().IsArray ) return ((IEnumerable ) value).Cast<object>().Select(x => x.ToString()).ToList();
                 return DataType.ToList(value);
             }

             if (value == null) value = "(null)";
             // throw exception
             throw new Rulez.RulezException(Rulez.RulezException.Types.ValueNotConvertible, arguments: new object[] { value, "list" });
         }
        /// <summary>
        /// converts a string of "|aa|bb|" to an array {"aa", "bb"}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
         public  static String[] ToArray(String input)
        {
            if (String.IsNullOrWhiteSpace (input))
            {
                return new String[0];
            }
            else
            {
                return  input.Split (ConstDelimiter ).Where(x =>  !String.IsNullOrEmpty (x) && !x.Contains (ConstDelimiter)  ).ToArray ();
            }
        }
        /// <summary>
        /// converts a string of "|aa|bb|" to a list {"aa", "bb"}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<String> ToList(String input)
        {
            if (String.IsNullOrWhiteSpace (input))
            {
                return new List<String> ();
            }
            else
            {
                return  input.Split (ConstDelimiter ).Where(x =>  !String.IsNullOrEmpty (x) && !x.Contains (ConstDelimiter)  ).ToList ();
            }
        }
        /// <summary>
        /// returns a string representation of an enumerable in "|aa|bb|cc|"
        /// returns String.Empty if IEnumerable is empty
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String ToString(IEnumerable input)
        {
            String result = String.Empty + ConstDelimiter ;
            foreach (var e in input)
            {
                if (e != null) result += e.ToString();
            }
            result += ConstDelimiter;

            if (result != String.Empty + ConstDelimiter + ConstDelimiter) return result;
            return String.Empty;
        }
    }
    /// <summary>
    /// ConverterHelpers
    /// </summary>
    public class Converter
    {
        public static string Array2StringList(object[] input, char delimiter = ',') {
        int i;
        // Warning!!! Optional parameters not supported
        if (input != null) {
            string aStrValue = String.Empty;
            for (i =0; (i <= input.GetUpperBound (1)); i++) {
                if ((i == 0)) {
                    aStrValue = input[i].ToString();
                }
                else {
                    aStrValue += delimiter + input[i].ToString();
                }
            }
            return aStrValue;
        }
        else {
            return String.Empty;
        }
    }

         public static string Enumerable2StringList(IEnumerable input, char delimiter =',') {
        string aStrValue = String.Empty;
        // Warning!!! Optional parameters not supported
        if ((input == null)) {
            return String.Empty;
        }
        foreach (var anElement in input) {
            string s;
            if ((anElement == null)) {
                s = String.Empty;
            }
            else {
                s = anElement.ToString();
            }
            if ((aStrValue == String.Empty)) {
                aStrValue = s;
            }
            else {
                aStrValue += delimiter + s;
            }
        }
        return aStrValue;
    }

        public static String ToString(object anObject)
        {
            if (anObject == null) return String.Empty;

            // convert inenumerables and arrays
            if ((anObject.GetType().IsArray) || (anObject .GetType().IsAssignableFrom (typeof(IEnumerable ))))
            {
                String aString = String.Empty + DataType.ConstDelimiter  ;
                foreach (Object anItem in (anObject as IEnumerable ))            if (anItem != null) aString += anItem.ToString();
                aString += DataType.ConstDelimiter;
                return aString;
            }

            // convert all others
            return anObject.ToString();
        }
        /// <summary>
        /// return a timestamp in the localTime
        /// </summary>
        /// <param name="datevalue"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string DateTime2LocaleDateTimeString(DateTime datevalue)
        {
            string formattimestamp = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
            return datevalue.ToString (formattimestamp);
        }

        /// <summary>
        /// return a date in the date localTime
        /// </summary>
        /// <param name="datevalue"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string DateTime2UniversalDateTimeString(DateTime datevalue)
        {
            string formattimestamp = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.UniversalSortableDateTimePattern;
            return datevalue.ToString(formattimestamp);
        }
        /// <summary>
        /// return a date in the date localTime
        /// </summary>
        /// <param name="datevalue"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string Date2LocaleShortDateString(System.DateTime datevalue)
        {
            string formattimestamp = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            return datevalue.ToString (formattimestamp);
        }
        /// <summary>
        /// return a date in the date localTime
        /// </summary>
        /// <param name="datevalue"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string Time2LocaleShortTimeString(DateTime timevalue)
        {
            string formattimestamp = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
            return timevalue.ToString(formattimestamp);
        }
        /// <summary>
        /// translates an hex integer to argb presentation integer RGB(FF,00,00) = FF but integer = FF0000
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static long Int2ARGB(long value)
        {
            long red = 0;
            long green = 0;
            long blue = 0;
            blue = value & 0xffL;
            green = value / 0x100L & 0xffL;
            red = value / 0x10000 & 0xffL;
            return blue * Convert.ToUInt32 (Math.Pow(255, 2)) + green * 255 + red;
        }

        /// <summary>
        /// returns a color value in rgb to system.drawing.color
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static System.Drawing.Color RGB2Color(long value)
        {
            long red = 0;
            long green = 0;
            long blue = 0;
            red = value & 0xffL;
            green = value / 0x100L & 0xffL;
            blue = value / 0x10000 & 0xffL;
            return System.Drawing.Color.FromArgb(red: Convert.ToInt32(red), green: Convert.ToInt32(green), blue: Convert.ToInt32(blue));
        }

        /// <summary>
        /// returns a color value to hexadecimal (bgr of rgb) 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static long Color2RGB(System.Drawing.Color color)
        {
            long red = 0;
            long green = 0;
            long blue = 0;
            blue = color.B;
            green = color.G;
            red = color.R;
            return blue * Convert.ToUInt32 (Math.Pow(255, 2)) + green * 255 + red;
        }

    }
}
