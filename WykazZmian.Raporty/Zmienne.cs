using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Novacode;

namespace WykazZmian.Raporty
{
    public static class Zmienne
    {
        public static Dictionary<string, int> header = new Dictionary<string, int>();

        public static DocX kerg(this DocX doc, object value) { return doc.podstaw("[KERG]", value); }
        public static DocX jewTeryt(this DocX doc, object value) { return doc.podstaw("[JEW_TERYT]", value); }
        public static DocX jewNazwa(this DocX doc, object value) { return doc.podstaw("[JEW_NAZWA]", value); }
        public static DocX obrTeryt(this DocX doc, object value) { return doc.podstaw("[OBR_TERYT]", value); }
        public static DocX obrNazwa(this DocX doc, object value) { return doc.podstaw("[OBR_NAZWA]", value); }
        public static DocX podstaw(this DocX doc, string key, object value) { doc.ReplaceText(key, value.ToString()); return doc; }

        public static Row lp(this Row row, object value) { return row.podstaw("[LP]", value); }
        public static Row jrg(this Row row, object value) { return row.podstaw("[JRG]", value); }
        public static Row kw(this Row row, object value) { return row.podstaw("[KW]", value); }
        public static Row id(this Row row, object value) { return row.podstaw("[ID]", value); }
        public static Row nr(this Row row, object value) { return row.podstaw("[NR]", value); }
        public static Row pow(this Row row, object value) { return row.podstaw("[POW]", value); }
        public static Row ofu(this Row row, object value) { return row.podstaw("[OFU]", value); }
        public static Row ozu(this Row row, object value) { return row.podstaw("[OZU]", value); }
        public static Row ozk(this Row row, object value) { return row.podstaw("[OZK]", value); }
        public static Row uz(this Row row, object value) { return row.podstaw("[UZ]", value); }
        public static Row idNowy(this Row row, object value) { return row.podstaw("[IDn]", value); }
        public static Row nrNowy(this Row row, object value) { return row.podstaw("[NRn]", value); }
        public static Row powNowy(this Row row, object value) { return row.podstaw("[POWn]", value); }
        public static Row ofuNowy(this Row row, object value) { return row.podstaw("[OFUn]", value); }
        public static Row ozuNowy(this Row row, object value) { return row.podstaw("[OZUn]", value); }
        public static Row ozkNowy(this Row row, object value) { return row.podstaw("[OZKn]", value); }
        public static Row uzNowy(this Row row, object value) { return row.podstaw("[UZn]", value); }
        public static Row uzDelta(this Row row, object value) { return row.podstaw("[UZr]", value); }
        public static Row wl(this Row row, object value) { return row.podstaw("[WL]", value); }
        public static Row uwaga(this Row row, object value) { return row.podstaw("[UWAGA]", value); }

        public static Row podstaw(this Row row, string key, object value)
        {
            if (!header.ContainsKey(key)) return row;
            int index = header[key];
            Cell cell = row.Cells[index];
            Paragraph p = cell.Paragraphs.First();
            p.ReplaceText(key, value.ToString());
            return row;
        }

        public static Cell podstaw(this Cell cell, string key, object value)
        {
            cell.ReplaceText(key, value.ToString()); return cell;
        }
        
        public static Row pogrub(this Row row)
        {
            foreach (var cell in row.Cells) cell.Paragraphs.First().Bold();
            return row;
        }

        public static Row ukryjZmienne(this Row row)
        {
            foreach (var cell in row.Cells) 
                if (cell.toZmienna())
                    cell.Paragraphs.First().Hide();
            return row;
        }

        public static bool toZmienna(this Cell cell)
        {
            string key = cell.Paragraphs.First().Text;
            return key.StartsWith("[") && key.EndsWith("]");
        }

    }
}
