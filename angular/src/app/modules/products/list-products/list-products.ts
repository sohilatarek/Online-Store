import { ListService, PagedResultDto, PermissionService } from '@abp/ng.core';
import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { CategoriesService, CategoryDto } from 'src/app/proxy/categories';
import { ProductDto, ProductsService } from 'src/app/proxy/products';

@Component({
  selector: 'app-list-products',
  imports: [],
  templateUrl: './list-products.html',
  styleUrl: './list-products.scss'
})
export class ListProducts {
  //products: ProductDto[] = [];
  searchForm: FormGroup;
  categories: CategoryDto[] = [];
  // count: number;
  products$: Observable<PagedResultDto<ProductDto>>;
  canCreate: boolean;

  constructor(private productsService: ProductsService,
    private router: Router,
    private formBuilder: FormBuilder,
    private categoriesService: CategoriesService,
    /*  public readonly list: ListService<GetProductListDto>, */
    private permissionService: PermissionService
  ) {
    this.buildForm();
  }
  ngOnInit(): void {
    this.categoriesService.getList({ maxResultCount: 100, skipCount: 0 }).subscribe(res => {
      this.categories = res.items;
    });
    this.searchProducts();

    this.canCreate = this.permissionService.getGrantedPolicy('Demo1.Products.CreateEdit');

  }

  buildForm() {
    this.searchForm = this.formBuilder.group({
      filter: new FormControl(''),
      categoryId: new FormControl(null),
      maxResultCount: new FormControl(50, Validators.required),
      // skipCount: new FormControl(0, Validators.required)
    });
  }


  addProduct() {
    this.router.navigateByUrl('/products/add');
  }

  searchProducts() {
    // A function that gets query and returns an observable
    const productStreamCreator = query => this.productsService.getList({ ...query, ...this.searchForm.value });

    /* this.products$ = this.list.hookToQuery(productStreamCreator); // Subscription is auto-cleared on destroy. */
  }
}
