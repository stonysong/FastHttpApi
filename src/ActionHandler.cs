﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.FastHttpApi
{
    class ActionHandler
    {
        public ActionHandler(object controller, System.Reflection.MethodInfo method)
        {
            Parameters = new List<ParameterBinder>();
            mMethod = method;
            Controller = controller;
            LoadParameter();
        }

        private System.Reflection.MethodInfo mMethod;

        public object Controller { get; set; }

        private void LoadParameter()
        {
            foreach (System.Reflection.ParameterInfo pi in mMethod.GetParameters())
            {
                ParameterBinder pb = new DefaultParameter();
                ParameterBinder[] customPB = (ParameterBinder[])pi.GetCustomAttributes(typeof(ParameterBinder), false);
                if (customPB != null && customPB.Length > 0)
                {
                    pb = customPB[0];
                }
                else if (pi.ParameterType == typeof(string))
                {
                    pb = new StringParameter();
                }
                else if (pi.ParameterType == typeof(DateTime))
                {
                    pb = new DateTimeParameter();
                }

                else if (pi.ParameterType == typeof(Decimal))
                {
                    pb = new DecimalParameter();
                }
                else if (pi.ParameterType == typeof(float))
                {
                    pb = new FloatParameter();
                }
                else if (pi.ParameterType == typeof(double))
                {
                    pb = new DoubleParameter();
                }
                else if (pi.ParameterType == typeof(short))
                {
                    pb = new ShortParameter();
                }
                else if (pi.ParameterType == typeof(int))
                {
                    pb = new IntParameter();
                }
                else if (pi.ParameterType == typeof(long))
                {
                    pb = new LongParameter();
                }
                else if (pi.ParameterType == typeof(ushort))
                {
                    pb = new UShortParameter();
                }
                else if (pi.ParameterType == typeof(uint))
                {
                    pb = new UIntParameter();
                }
                else if (pi.ParameterType == typeof(ulong))
                {
                    pb = new ULongParameter();
                }
                else if (pi.ParameterType == typeof(HttpRequest))
                {
                    pb = new RequestParameter();
                }
                else if (pi.ParameterType == typeof(HttpResponse))
                {
                    pb = new ResponseParameter();
                }
                else
                {


                    if (pi.ParameterType.GetInterface("BeetleX.HttpExtend.IBodyFlag") != null)
                    {
                        pb = new BodyParameter();
                    }
                    else
                    {
                        pb = new DefaultParameter();
                    }
                }
                pb.Name = pi.Name;
                pb.Type = pi.ParameterType;
                Parameters.Add(pb);
            }
        }

        public List<ParameterBinder> Parameters { get; set; }

        private object[] GetValues(HttpRequest request, HttpResponse response)
        {
            object[] parameters = new object[Parameters.Count];
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = Parameters[i].GetValue(request, response);

            }
            return parameters;
        }

        public object Invoke(HttpRequest request, HttpResponse response)
        {
            object[] parameters = GetValues(request, response);
            return mMethod.Invoke(Controller, parameters);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class ParameterBinder : Attribute
    {
        public Type Type { get; internal set; }

        public string Name { get; internal set; }

        public abstract object GetValue(HttpRequest request, HttpResponse response);
    }

    class IntParameter : ParameterBinder
    {
        public override object GetValue(HttpRequest request, HttpResponse response)
        {
            int result;
            request.QueryString.TryGetInt(this.Name, out result);
            return result;
        }
    }

    class ShortParameter : ParameterBinder
    {
        public override object GetValue(HttpRequest request, HttpResponse response)
        {
            short result;
            request.QueryString.TryGetShort(this.Name, out result);
            return result;
        }
    }

    class LongParameter : ParameterBinder
    {
        public override object GetValue(HttpRequest request, HttpResponse response)
        {
            long result;
            request.QueryString.TryGetLong(this.Name, out result);
            return result;
        }
    }

    class UIntParameter : ParameterBinder
    {
        public override object GetValue(HttpRequest request, HttpResponse response)
        {
            uint result;
            request.QueryString.TryGetUInt(this.Name, out result);
            return result;
        }
    }

    class UShortParameter : ParameterBinder
    {
        public override object GetValue(HttpRequest request, HttpResponse response)
        {
            ushort result;
            request.QueryString.TryGetUShort(this.Name, out result);
            return result;
        }
    }

    class ULongParameter : ParameterBinder
    {
        public override object GetValue(HttpRequest request, HttpResponse response)
        {
            ulong result;
            request.QueryString.TryGetULong(this.Name, out result);
            return result;
        }
    }

    class FloatParameter : ParameterBinder
    {
        public override object GetValue(HttpRequest request, HttpResponse response)
        {
            float result;
            request.QueryString.TryGetFloat(this.Name, out result);
            return result;
        }
    }

    class DoubleParameter : ParameterBinder
    {
        public override object GetValue(HttpRequest request, HttpResponse response)
        {
            double result;
            request.QueryString.TryGetDouble(this.Name, out result);
            return result;
        }
    }

    class StringParameter : ParameterBinder
    {
        public override object GetValue(HttpRequest request, HttpResponse response)
        {
            string result;
            request.QueryString.TryGetString(this.Name, out result);
            return result;
        }
    }

    class DecimalParameter : ParameterBinder
    {
        public override object GetValue(HttpRequest request, HttpResponse response)
        {
            Decimal result;
            request.QueryString.TryGetDecimal(this.Name, out result);
            return result;
        }
    }

    class DateTimeParameter : ParameterBinder
    {
        public override object GetValue(HttpRequest request, HttpResponse response)
        {
            DateTime result;
            request.QueryString.TryGetDateTime(this.Name, out result);
            return result;
        }
    }

    public class BodyParameter : ParameterBinder
    {
        public override object GetValue(HttpRequest request, HttpResponse response)
        {
            return request.GetBody(this.Type);
        }
    }

    class RequestParameter : ParameterBinder
    {
        public override object GetValue(HttpRequest request, HttpResponse response)
        {
            return request;
        }
    }



    class ResponseParameter : ParameterBinder
    {
        public override object GetValue(HttpRequest request, HttpResponse response)
        {
            return response;
        }
    }

    class DefaultParameter : ParameterBinder
    {
        public override object GetValue(HttpRequest request, HttpResponse response)
        {
            return null;
        }
    }

}