using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Items
{
    public class FirearmsItem : BaseItem
    {
        public enum FirearmsType
        {
            AssultRefile,
            HandGun,
        }
        public FirearmsType currentFirearmsType;
        /// <summary>
        /// 相当于匹配武器的种类
        /// </summary>
        public string armsName;
    }
}