import type { EntityDto, FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface AdjustStockDto {
  quantityChange: number;
  reason?: string;
}

export interface BulkStockItem {
  productId: number;
  stockQuantity: number;
}

export interface BulkUpdateStockDto {
  items: BulkStockItem[];
}

export interface CheckStockInput {
  items: StockCheckItem[];
}

export interface CreateUpdateProductDto extends EntityDto<number> {
  nameAr?: string;
  nameEn?: string;
  descriptionAr?: string;
  descriptionEn?: string;
  categoryId: number;
  sku?: string;
  price: number;
  stockQuantity: number;
  isActive: boolean;
  isPublished: boolean;
}

export interface ProductDto extends FullAuditedEntityDto<number> {
  nameAr?: string;
  nameEn?: string;
  descriptionAr?: string;
  descriptionEn?: string;
  categoryId: number;
  categoryNameAr?: string;
  categoryNameEn?: string;
  categoryDescriptionAr?: string;
  categoryDescriptionEn?: string;
  sku?: string;
  price: number;
  stockQuantity: number;
  isActive: boolean;
  isPublished: boolean;
  inventoryValue: number;
  isOutOfStock: boolean;
  isLowStock: boolean;
}

export interface StockCheckItem {
  productId: number;
  quantity: number;
}

export interface StockCheckItemResultDto {
  productId: number;
  productName?: string;
  sku?: string;
  requestedQuantity: number;
  availableQuantity: number;
  isAvailable: boolean;
  message?: string;
}

export interface StockCheckResultDto {
  allAvailable: boolean;
  items: StockCheckItemResultDto[];
}

export interface UpdateStockDto {
  stockQuantity: number;
}

export interface GetProductsInput extends PagedAndSortedResultRequestDto {
  categoryId?: number;
  isActive?: boolean;
  isPublished?: boolean;
  searchTerm?: string;
  minPrice?: number;
  maxPrice?: number;
  isLowStock?: boolean;
  lowStockThreshold?: number;
  isOutOfStock?: boolean;
}
