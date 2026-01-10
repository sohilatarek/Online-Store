import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListCategories } from './list-categories/list-categories';
import { AddCategory } from './add-category/add-category';
import { authGuard, permissionGuard } from '@abp/ng.core';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: ListCategories,
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'OnlineStore.Categories' }
  },
  {
    path: 'add',
    pathMatch: 'full',
    component: AddCategory,
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'OnlineStore.Categories.Create' }
  },
  {
    path: 'edit/:id',
    pathMatch: 'full',
    component: AddCategory,
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'OnlineStore.Categories.Edit' }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CategoriesRoutingModule { }
