// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations
{
    public class Pagination : MonoBehaviour
    {
        public    GameObject  PaginationDotPrefab;
        public    Color       ActiveColor;
        public    Color       InactiveColor;
        protected List<Image> _images;

        public virtual void InitializePagination(int numberOfPages)
        {
            this._images = new();
            for (var i = 0; i < numberOfPages; i++)
            {
                var dotPrefab = Instantiate(this.PaginationDotPrefab);
                dotPrefab.transform.SetParent(this.transform);
                dotPrefab.name = "PaginationDot" + i;
                this._images.Add(dotPrefab.GetComponent<Image>());
            }
            foreach (var image in this._images)
            {
                image.color                       = this.InactiveColor;
                image.rectTransform.localScale    = Vector3.one;
                image.rectTransform.localPosition = Vector3.zero;
                image.SetNativeSize();
            }
        }

        public virtual void SetCurrentPage(int numberOfPages, int currentPage)
        {
            for (var i = 0; i < numberOfPages; i++)
            {
                if (i == currentPage)
                    this._images[i].color = this.ActiveColor;
                else
                    this._images[i].color = this.InactiveColor;
            }
        }
    }
}