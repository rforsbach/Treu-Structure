using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    /// <summary>
    /// Clase que representa los grados de libertad de un nodo. 
    /// Esto incluye la representación de resortes y restricciones.
    /// </summary>
    [Serializable]
    public class JointDOF {
        private float[] springValues = null;
        private byte restraints = 0;
        private byte springs = 0;
    
        public enum DofType {
            Free,
            Restrained,
            Spring
        }

        public JointDOF(){
        }

        public JointDOF(bool allRestrained) {
            if (allRestrained)
                restraints = 0x3F;
            else
                restraints = 0;
        }

        internal byte AllRestraints {
            get { return restraints; }
        }

        internal byte AllSprings {
            get { return springs; }
        }

        internal short AllDOF
        {
            get { return (short)(springs << 8 | restraints); }
        }

        public JointDOF Clone()
        {
            JointDOF ret = new JointDOF();
            ret.restraints = restraints;
            ret.springs = springs;
            ret.springValues = (springValues == null) ? null : (float[])springValues.Clone();
            return ret;
        }

        public DofType T1
        {
            get
            {
                if ((restraints & 1) != 0)
                    return DofType.Restrained;
                else if ((springs & 1) != 0)
                    return DofType.Spring;
                else
                    return DofType.Free;
            }
            set
            {
                switch (value)
                {
                    case DofType.Free:
                        springs = (byte)(((int)springs) & ~1);
                        restraints = (byte)(((int)restraints) & ~1);
                        break;
                    case DofType.Restrained:
                        springs = (byte)(((int)springs) & ~1);
                        restraints |= 1;
                        break;
                    case DofType.Spring:
                        springs |= 1;
                        restraints = (byte)(((int)restraints) & ~1);
                        break;
                }
            }
        }

        public DofType T2
        {
            get
            {
                if ((restraints & 2) != 0)
                    return DofType.Restrained;
                else if ((springs & 2) != 0)
                    return DofType.Spring;
                else
                    return DofType.Free;
            }
            set
            {
                switch (value)
                {
                    case DofType.Free:
                        springs = (byte)(((int)springs) & ~2);
                        restraints = (byte)(((int)restraints) & ~2);
                        break;
                    case DofType.Restrained:
                        springs = (byte)(((int)springs) & ~2);
                        restraints |= 2;
                        break;
                    case DofType.Spring:
                        springs |= 2;
                        restraints = (byte)(((int)restraints) & ~2);
                        break;
                }
            }
        }

        public DofType T3
        {
            get
            {
                if ((restraints & 4) != 0)
                    return DofType.Restrained;
                else if ((springs & 4) != 0)
                    return DofType.Spring;
                else
                    return DofType.Free;
            }
            set
            {
                switch (value)
                {
                    case DofType.Free:
                        springs = (byte)(((int)springs) & ~4);
                        restraints = (byte)(((int)restraints) & ~4);
                        break;
                    case DofType.Restrained:
                        springs = (byte)(((int)springs) & ~4);
                        restraints |= 4;
                        break;
                    case DofType.Spring:
                        springs |= 4;
                        restraints = (byte)(((int)restraints) & ~4);
                        break;
                }
            }
        }

        public DofType R1
        {
            get
            {
                if ((restraints & 8) != 0)
                    return DofType.Restrained;
                else if ((springs & 8) != 0)
                    return DofType.Spring;
                else
                    return DofType.Free;
            }
            set
            {
                switch (value)
                {
                    case DofType.Free:
                        springs = (byte)(((int)springs) & ~8);
                        restraints = (byte)(((int)restraints) & ~8);
                        break;
                    case DofType.Restrained:
                        springs = (byte)(((int)springs) & ~8);
                        restraints |= 8;
                        break;
                    case DofType.Spring:
                        springs |= 8;
                        restraints = (byte)(((int)restraints) & ~8);
                        break;
                }
            }
        }

        public DofType R2
        {
            get
            {
                if ((restraints & 16) != 0)
                    return DofType.Restrained;
                else if ((springs & 16) != 0)
                    return DofType.Spring;
                else
                    return DofType.Free;
            }
            set
            {
                switch (value)
                {
                    case DofType.Free:
                        springs = (byte)(((int)springs) & ~16);
                        restraints = (byte)(((int)restraints) & ~16);
                        break;
                    case DofType.Restrained:
                        springs = (byte)(((int)springs) & ~16);
                        restraints |= 16;
                        break;
                    case DofType.Spring:
                        springs |= 16;
                        restraints = (byte)(((int)restraints) & ~16);
                        break;
                }
            }
        }

        public DofType R3
        {
            get
            {
                if ((restraints & 32) != 0)
                    return DofType.Restrained;
                else if ((springs & 32) != 0)
                    return DofType.Spring;
                else
                    return DofType.Free;
            }
            set
            {
                switch (value)
                {
                    case DofType.Free:
                        springs = (byte)(((int)springs) & ~32);
                        restraints = (byte)(((int)restraints) & ~32);
                        break;
                    case DofType.Restrained:
                        springs = (byte)(((int)springs) & ~32);
                        restraints |= 32;
                        break;
                    case DofType.Spring:
                        springs |= 32;
                        restraints = (byte)(((int)restraints) & ~32);
                        break;
                }
            }
        }

        /// <summary>
        /// Guarda / Regresa una copia del arreglo de valores para los resortes, en
        /// el sistema de unidades correcto.
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SpringTranslation)]
        public float[] SpringValues
        {
            get
            {
                float[] tmp = new float[6];
                if (springValues != null)
                {
                    for (int i = 0; i < 3; i++)
                        tmp[i] = Model.Instance.UnitSystem.FromInternational(springValues[i], Canguro.Model.UnitSystem.Units.SpringTranslation);
                    for (int i = 3; i < 6; i++)
                        tmp[i] = Model.Instance.UnitSystem.FromInternational(springValues[i], Canguro.Model.UnitSystem.Units.SpringRotation);
                }
                return tmp;
            }
            set
            {
                if (springValues == null && value != null)
                    springValues = new float[6];
                for (int i = 0; i < 3; i++)
                    springValues[i] = Model.Instance.UnitSystem.ToInternational(value[i], Canguro.Model.UnitSystem.Units.SpringTranslation);
                for (int i = 3; i < 6; i++)
                    springValues[i] = Model.Instance.UnitSystem.ToInternational(value[i], Canguro.Model.UnitSystem.Units.SpringRotation);
                springs = 0;
                for (int i = 0; i < 6; i++)
                    if (springValues[i] > 0)
                        springs |= (byte)(1 << i);

                // If is spring with constant 0 or neg, it's not spring for each DoF
                for (int i = 0; i < 6; i++)
                    if (springValues[i] <= 0 && (springs & 1 << i) > 0)
                        springs ^= (byte)(1 << i);
            }
        }

        /// <summary>
        /// Propiedad de solo lectura para saber rápidamente si el nodo está restringido.
        /// </summary>
        public bool IsRestrained{
            get {
                return restraints != 0;
            }
        }

        /// <summary>
        /// Propiedad de solo lectura para saber rápidamente si el nodo está liberado.
        /// </summary>
        public bool IsFree {
            get {
                return ((restraints|springs)&0x3F)!= 0x3f; 
            }
        }


        /// <summary>
        /// Propiedad de solo lectura para saber rápidamente si el nodo tiene resortes.
        /// </summary>
        public bool IsSpring
        {
            get
            {
                return springs != 0;
            }
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder(12);
            for (int i = 0; i < 6; i++)
            {
                int mask = 1 << i;
                if ((restraints & mask) == 0)
                {
                    if ((springs & mask) == 0)
                        buf.Append("F ");
                    else
                    {
                        Canguro.Model.UnitSystem.Units unit = (i >= 3)? Canguro.Model.UnitSystem.Units.SpringRotation : Canguro.Model.UnitSystem.Units.SpringTranslation;
                        float spring = Model.Instance.UnitSystem.FromInternational(springValues[i], unit);
                        string format = "#.00";
                        buf.Append("S(" + spring.ToString(format) +")");
                    }
                }
                else
                    buf.Append("R ");
            }
            return buf.ToString();
        }
    }
}
