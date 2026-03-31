# Order API Guide

## Base URL

- Development: `https://localhost:5001`
- Alternative: `http://localhost:5000`

## Authentication

- Order APIs that create, update, delete, or get the current user's orders require `Authorization: Bearer <token>`.
- The backend reads `AccountId` from the JWT token, so the client does not need to send `accountId` in order requests.

## How order creation works

- An order is created from `ProductVariant` ids, not `Product` ids.
- Each item in `orderItems` must include `variantId` and `quantity`.
- The server fetches the variant price from the database.
- The server validates stock before creating the order.

## 1. Create order

**Endpoint**

```http
POST /api/order/create
Authorization: Bearer <token>
Content-Type: application/json
```

**Request**

```json
{
  "orderItems": [
    {
      "variantId": "11111111-1111-1111-1111-111111111111",
      "quantity": 2
    },
    {
      "variantId": "22222222-2222-2222-2222-222222222222",
      "quantity": 1
    }
  ]
}
```

**curl**

```bash
curl -X POST https://localhost:5001/api/order/create \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "orderItems": [
      {
        "variantId": "11111111-1111-1111-1111-111111111111",
        "quantity": 2
      },
      {
        "variantId": "22222222-2222-2222-2222-222222222222",
        "quantity": 1
      }
    ]
  }'
```

**Response**

```json
{
  "id": "9e4ef5e8-2c3d-4a69-8b1d-6e7a8f9c1234",
  "orderDate": "2026-03-31T09:15:22.123Z",
  "status": "Pending",
  "totalAmount": 650000,
  "items": [
    {
      "variantId": "11111111-1111-1111-1111-111111111111",
      "quantity": 2,
      "price": 250000
    },
    {
      "variantId": "22222222-2222-2222-2222-222222222222",
      "quantity": 1,
      "price": 150000
    }
  ]
}
```

## 2. Get my orders

**Endpoint**

```http
GET /api/order/get-my
Authorization: Bearer <token>
```

**curl**

```bash
curl https://localhost:5001/api/order/get-my \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## 3. Get all orders

**Endpoint**

```http
GET /api/order/get-all
```

**curl**

```bash
curl https://localhost:5001/api/order/get-all
```

## 4. Get order by id

**Endpoint**

```http
GET /api/order/get-by-id/{id}
```

**curl**

```bash
curl https://localhost:5001/api/order/get-by-id/9e4ef5e8-2c3d-4a69-8b1d-6e7a8f9c1234
```

## 5. Update order

**Endpoint**

```http
PUT /api/order/update/{id}
Authorization: Bearer <token>
Content-Type: application/json
```

**Request**

```json
{
  "orderItems": [
    {
      "variantId": "11111111-1111-1111-1111-111111111111",
      "quantity": 3
    }
  ]
}
```

**curl**

```bash
curl -X PUT https://localhost:5001/api/order/update/9e4ef5e8-2c3d-4a69-8b1d-6e7a8f9c1234 \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "orderItems": [
      {
        "variantId": "11111111-1111-1111-1111-111111111111",
        "quantity": 3
      }
    ]
  }'
```

## 6. Delete order

**Endpoint**

```http
DELETE /api/order/delete/{id}
Authorization: Bearer <token>
```

**curl**

```bash
curl -X DELETE https://localhost:5001/api/order/delete/9e4ef5e8-2c3d-4a69-8b1d-6e7a8f9c1234 \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## 7. Get product variants for order creation

The frontend should load variants first, then send the selected `variantId` into the order API.

**Get all variants**

```http
GET /api/product-variant/get-all
```

```bash
curl https://localhost:5001/api/product-variant/get-all
```

**Get variants by product**

```http
GET /api/product-variant/get-by-product/{productId}
```

```bash
curl https://localhost:5001/api/product-variant/get-by-product/33333333-3333-3333-3333-333333333333
```

**Variant response example**

```json
[
  {
    "id": "11111111-1111-1111-1111-111111111111",
    "productId": "33333333-3333-3333-3333-333333333333",
    "productName": "Custom Phone Case",
    "sku": "CASE-RED-MATTE",
    "name": "Red Matte",
    "stockQuantity": 15,
    "imageUrl": "https://example.com/case-red.jpg",
    "color": "Red",
    "size": "M",
    "price": 250000
  }
]
```

## Common errors

- `User not authenticated.`: missing token or token does not contain a valid user id.
- `Order must contain at least one item.`: `orderItems` is null or empty.
- `Product variant with ID ... not found.`: invalid `variantId`.
- `Insufficient stock for variant ...`: requested quantity is greater than available stock.
