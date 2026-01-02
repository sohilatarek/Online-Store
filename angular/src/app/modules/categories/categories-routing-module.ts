import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListCategories } from './list-categories/list-categories';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: ListCategories

  }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CategoriesRoutingModule { }
