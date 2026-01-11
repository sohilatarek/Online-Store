import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddProduct } from './add-product/add-product';
import { ListProducts } from './list-products/list-products';

const routes: Routes = [{
  path: 'add',
  pathMatch: 'full',
  component: AddProduct
},
{
  path: 'edit/:id',
  pathMatch: 'full',
  component: AddProduct
},
{
  path: '',
  pathMatch: 'full',
  component: ListProducts
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductsRoutingModule { }
