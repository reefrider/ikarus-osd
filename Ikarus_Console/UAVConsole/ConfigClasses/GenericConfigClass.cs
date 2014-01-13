/* 
 * (c) 2010 Rafael Paz <rpaz@atc.us.es>
 *
 *************
 *
 *	This file is part of IKARUS_OSD.
 *
 *  IKARUS_OSD is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  IKARUS_OSD is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with IKARUS_OSD.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using UAVConsole.USBXpress;

namespace UAVConsole.ConfigClasses
{
    [StructLayout(LayoutKind.Sequential)]
    public class GenericConfigClass
    {

        public virtual void LoadDefaults()
        {
        }
        
        public virtual FieldInfo[] GetFields()
        {
            FieldInfo[] fields = this.GetType().GetFields();
            return fields;
        }

        public object getElement(int idx)
        {
            FieldInfo[] tmp = this.GetType().GetFields();
            if (idx < tmp.Length)
                return tmp[idx].GetValue(this);
            else
                return null;
        }

        public int getNumElements()
        {
            FieldInfo[] tmp = this.GetType().GetFields();
            return tmp.Length;
        }

        public void LoadFromXmlString(String XmlString)
        {
            Type tipobase = this.GetType();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(new StringReader(XmlString));

            XmlNodeList nodelist = xDoc.GetElementsByTagName(this.GetType().Name);

            if (nodelist.Count > 0)
            {
                XmlNode main_node = nodelist[0];
                this.LoadFromXmlAux(main_node);
            }
        }

        public void LoadFromXml(string FileName)
        {
            Type tipobase = this.GetType();
        
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(FileName);
            
            XmlNodeList nodelist = xDoc.GetElementsByTagName(this.GetType().Name);

            if (nodelist.Count > 0)
            {
                XmlNode main_node = nodelist[0];
                this.LoadFromXmlAux(main_node);
            }
        }

        void LoadFromXmlAux(XmlNode main_node)
        {
            Type tipobase = this.GetType();
            
            foreach (XmlNode node in main_node.ChildNodes)
            {
                
                FieldInfo tipo = this.GetType().GetField(node.Name);
                if (tipo != null)
                {
                    Object tmp;
                    
                    if (tipo.FieldType == typeof(byte))
                        tmp = byte.Parse(node.InnerText);
                    else if (tipo.FieldType == typeof(sbyte))
                        tmp = sbyte.Parse(node.InnerText);
                    else if (tipo.FieldType == typeof(float))
                        tmp = float.Parse(node.InnerText, CultureInfo.InvariantCulture);
                    else if (tipo.FieldType == typeof(Int16))
                        tmp = Int16.Parse(node.InnerText);
                    else if (tipo.FieldType == typeof(int))
                        tmp = int.Parse(node.InnerText);
                    else if (tipo.FieldType == typeof(string))
                        tmp = node.InnerText;
                    else if (tipo.FieldType.IsSubclassOf(typeof(GenericConfigClass)))
                    {
                        tmp = Activator.CreateInstance(tipo.FieldType);
                        ((GenericConfigClass)tmp).LoadFromXmlAux(node);
                    }
                    else
                        tmp = null;
                    tipobase.GetField(node.Name).SetValue(this, tmp);
                }
            }
        }

        public void SaveToXml(string FileName)
        {
            XmlTextWriter writer = new XmlTextWriter(FileName, null);
            SaveToXml(writer);
            writer.Close();
        }

        public void SaveToXml(XmlTextWriter writer)
        {
            //Write the root element
            Type tipobase = this.GetType();
            writer.WriteStartElement(tipobase.Name);

            SaveToXmlAux(this, writer);

            // end the root element
            writer.WriteEndElement();
        }

        void SaveToXmlAux(Object obj, XmlTextWriter writer)
        {
            FieldInfo[] fields = obj.GetType().GetFields();

            foreach (FieldInfo field in fields)
            {
                Type tipo = field.FieldType;
                System.Console.WriteLine("" + field.Name + "," + field.FieldType + "," + field.FieldType.BaseType);

                if (field.FieldType == typeof(byte))
                    writer.WriteElementString(field.Name, "" + field.GetValue(obj));
                else if (field.FieldType == typeof(sbyte))
                    writer.WriteElementString(field.Name, "" + field.GetValue(obj));
                else if (field.FieldType == typeof(Int16))
                    writer.WriteElementString(field.Name, "" + field.GetValue(obj));
                else if (field.FieldType == typeof(Int32))
                    writer.WriteElementString(field.Name, "" + field.GetValue(obj));
                else if (field.FieldType == typeof(float))
                    writer.WriteElementString(field.Name, ((float)field.GetValue(obj)).ToString(CultureInfo.InvariantCulture));
                else if (field.FieldType == typeof(string))
                    writer.WriteElementString(field.Name, ""+field.GetValue(obj));
                else if (!field.FieldType.IsArray)
                {
                    writer.WriteStartElement(field.Name);
                    SaveToXmlAux(field.GetValue(obj), writer);
                    writer.WriteEndElement();
                }
            }
        }


        public virtual int size_bytes()
        {
            int size = 0;
            FieldInfo[] fields = GetType().GetFields();
            foreach (FieldInfo field in fields)
            {
                Type tipo = field.FieldType;

                if (tipo == typeof(byte))
                    size += 1;
                else if (tipo == typeof(sbyte))
                    size += 1;
                else if (tipo == typeof(Int16))
                    size += 2;
                else if (tipo == typeof(Int32))
                    size += 4;
                else if (tipo == typeof(float))
                    size += 4;
                else if (tipo == typeof(string))
                {
                    size += 16;
                }
                else if (tipo.IsSubclassOf(typeof(GenericConfigClass)))
                {
                    GenericConfigClass tmp = (GenericConfigClass)Activator.CreateInstance(tipo);
                    size += tmp.size_bytes();
                }
                else
                    size+=0;
            }
            return size;
        }


        public virtual byte[] ToByteArray()
        {
            byte[] buffer = new byte[size_bytes()];
            int i = 0;
            ToByteArrayAux(buffer, ref i);
            return buffer;
        }

        protected virtual void ToByteArrayAux(byte[] buffer, ref int i)
        {
            FieldInfo[] fields = this.GetType().GetFields();

            foreach (FieldInfo field in fields)
            {
                Type tipo = field.FieldType;

                if (tipo == typeof(byte))
                {
                    buffer[i] = (byte)field.GetValue(this);
                    i += 1;
                }
                else if (tipo == typeof(sbyte))
                {
                    sbyte tmp = (sbyte)field.GetValue(this);
                    buffer[i] = (byte)tmp;
                    i += 1;
                }
                else if (tipo == typeof(Int16))
                {

                    USBXpress.USBXpress.toarray(buffer, ref i, (Int16)field.GetValue(this));
                    //i += 2;
                }
                else if (tipo == typeof(Int32))
                {
                    USBXpress.USBXpress.toarray(buffer, ref i, (Int32)field.GetValue(this));
                    //i += 4;
                }
                else if (tipo == typeof(float))
                {
                    USBXpress.USBXpress.toarray(buffer, ref i, (float)field.GetValue(this));
                    //i += 4;
                }
                else if (tipo == typeof(string))
                {
                    string cad = (string)field.GetValue(this);
                    for (int j = 0; j < 16; j++)
                    {
                        if (j < cad.Length)
                            buffer[i + j] = (byte)cad[j];
                        else
                            buffer[i + j] = (byte)0;
                    }
                    i += 16;
                }
                else if (tipo.IsSubclassOf(typeof(GenericConfigClass)))
                {
                    GenericConfigClass tmp = (GenericConfigClass)field.GetValue(this);
                    tmp.ToByteArrayAux(buffer, ref i);
                }
            }
        }

        public virtual void FromByteArray(byte []buffer)
        {
            int i = 0;
            this.FromByteArrayAux(buffer, ref i);
        }

        protected virtual void FromByteArrayAux(byte[] buffer, ref int i)
        {
            FieldInfo[] fields = this.GetType().GetFields();

            foreach (FieldInfo field in fields)
            {
                Type tipo = field.FieldType;

                if (tipo == typeof(byte))
                {
                    field.SetValue(this, buffer[i]);
                    i += 1;
                }
                else if (tipo == typeof(sbyte))
                {
                    sbyte tmp = (sbyte)buffer[i];
                    field.SetValue(this, tmp);
                    i += 1;
                }
                else if (tipo == typeof(Int16))
                {
                    Int16 tmp = USBXpress.USBXpress.toint16(buffer, ref i);
                    field.SetValue(this, tmp);
                    //i += 2;
                }
                else if (tipo == typeof(Int32))
                {
                    Int32 tmp = USBXpress.USBXpress.toint32(buffer, ref i);
                    field.SetValue(this, tmp);
                    //i += 4;
                }
                else if (tipo == typeof(float))
                {
                    float tmp = USBXpress.USBXpress.tofloat(buffer, ref i);
                    field.SetValue(this, tmp);
                    //i += 4;
                }
                else if (tipo == typeof(string))
                {
                    string cad = "";
                    for (int j = 0; j < 16 && buffer[i+j]!=0; j++)
                    {
                        cad += (char)buffer[i + j];
                    }
                    field.SetValue(this, cad);
                    i += 16;
                }
                else if (tipo.IsSubclassOf(typeof(GenericConfigClass)))
                {
                    GenericConfigClass tmp = (GenericConfigClass) Activator.CreateInstance(tipo);
                    tmp.FromByteArrayAux(buffer, ref i);
                    field.SetValue(this, tmp);
                }
            }
        }
    }
}
