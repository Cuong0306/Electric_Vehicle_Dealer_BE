using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ElectricVehicleDealer.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "brand",
                columns: table => new
                {
                    brand_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brand_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    website = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    founder_year = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("brand_pkey", x => x.brand_id);
                });

            migrationBuilder.CreateTable(
                name: "customer",
                columns: table => new
                {
                    customer_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    create_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    license_up = table.Column<string>(type: "text", nullable: true),
                    license_down = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("customer_pkey", x => x.customer_id);
                });

            migrationBuilder.CreateTable(
                name: "store",
                columns: table => new
                {
                    store_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    store_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    promotion_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("store_pkey", x => x.store_id);
                });

            migrationBuilder.CreateTable(
                name: "staff",
                columns: table => new
                {
                    staff_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    password = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    position = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    brand_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("staff_pkey", x => x.staff_id);
                    table.ForeignKey(
                        name: "FK_staff_brand_brand_id",
                        column: x => x.brand_id,
                        principalTable: "brand",
                        principalColumn: "brand_id");
                });

            migrationBuilder.CreateTable(
                name: "vehicle",
                columns: table => new
                {
                    vehicle_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brand_id = table.Column<int>(type: "integer", nullable: false),
                    model_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    image_urls = table.Column<string[]>(type: "text[]", nullable: false),
                    version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    year = table.Column<int>(type: "integer", nullable: true),
                    color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    price = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: true),
                    battery_capacity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    range_per_charge = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    warranty_period = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    seating_capacity = table.Column<int>(type: "integer", nullable: true),
                    transmission = table.Column<string>(type: "text", nullable: true),
                    airbags = table.Column<int>(type: "integer", nullable: true),
                    horsepower = table.Column<int>(type: "integer", nullable: true),
                    vehicle_type = table.Column<string>(type: "text", nullable: true),
                    trunk_capacity = table.Column<int>(type: "integer", nullable: true),
                    daily_driving_limit = table.Column<int>(type: "integer", nullable: true),
                    screen = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    seat_material = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    interior_material = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    air_conditioning = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    speaker_system = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    in_vehicle_cabinet = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    length_mm = table.Column<int>(type: "integer", nullable: true),
                    width_mm = table.Column<int>(type: "integer", nullable: true),
                    height_mm = table.Column<int>(type: "integer", nullable: true),
                    wheels = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    headlights = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    taillights = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    frame_chassis = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    door_count = table.Column<int>(type: "integer", nullable: true),
                    glass_windows = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    mirrors = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cameras = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_allocation = table.Column<bool>(type: "boolean", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("vehicle_pkey", x => x.vehicle_id);
                    table.ForeignKey(
                        name: "vehicle_brand_id_fkey",
                        column: x => x.brand_id,
                        principalTable: "brand",
                        principalColumn: "brand_id");
                });

            migrationBuilder.CreateTable(
                name: "agreement",
                columns: table => new
                {
                    agreement_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    agreement_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    terms_and_conditions = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    file_url = table.Column<string>(type: "text", nullable: true),
                    store_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("agreement_pkey", x => x.agreement_id);
                    table.ForeignKey(
                        name: "FK_agreement_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "store_id");
                    table.ForeignKey(
                        name: "agreement_customer_id_fkey",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "customer_id");
                });

            migrationBuilder.CreateTable(
                name: "dealer",
                columns: table => new
                {
                    dealer_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    password = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    store_id = table.Column<int>(type: "integer", nullable: true),
                    role = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("dealer_pkey", x => x.dealer_id);
                    table.ForeignKey(
                        name: "dealer_store_id_fkey",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "store_id");
                });

            migrationBuilder.CreateTable(
                name: "promotion",
                columns: table => new
                {
                    promotion_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    discount_percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    store_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("promotion_pkey", x => x.promotion_id);
                    table.ForeignKey(
                        name: "FK_promotion_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "store_id");
                });

            migrationBuilder.CreateTable(
                name: "store_customer",
                columns: table => new
                {
                    store_id = table.Column<int>(type: "integer", nullable: false),
                    customer_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_customer", x => new { x.store_id, x.customer_id });
                    table.ForeignKey(
                        name: "FK_store_customer_customer_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "customer_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_store_customer_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "store_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "storage",
                columns: table => new
                {
                    storage_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BrandId = table.Column<int>(type: "integer", nullable: true),
                    vehicle_id = table.Column<int>(type: "integer", nullable: true),
                    store_id = table.Column<int>(type: "integer", nullable: true),
                    quantity_available = table.Column<int>(type: "integer", nullable: true),
                    last_updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("storage_pkey", x => x.storage_id);
                    table.ForeignKey(
                        name: "storage_store_id_fkey",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "store_id");
                    table.ForeignKey(
                        name: "storage_vehicle_id_fkey",
                        column: x => x.vehicle_id,
                        principalTable: "vehicle",
                        principalColumn: "vehicle_id");
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    dealer_id = table.Column<int>(type: "integer", nullable: false),
                    order_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    quantity = table.Column<int>(type: "integer", nullable: true),
                    total_price = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: true),
                    status = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    note = table.Column<string>(type: "text", nullable: true),
                    store_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("orders_pkey", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_orders_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "store_id");
                    table.ForeignKey(
                        name: "orders_customer_id_fkey",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "customer_id");
                    table.ForeignKey(
                        name: "orders_dealer_id_fkey",
                        column: x => x.dealer_id,
                        principalTable: "dealer",
                        principalColumn: "dealer_id");
                });

            migrationBuilder.CreateTable(
                name: "test_appointment",
                columns: table => new
                {
                    test_appointment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    vehicle_id = table.Column<int>(type: "integer", nullable: false),
                    dealer_id = table.Column<int>(type: "integer", nullable: false),
                    appointment_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status = table.Column<int>(type: "integer", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("test_appointment_pkey", x => x.test_appointment_id);
                    table.ForeignKey(
                        name: "test_appointment_customer_id_fkey",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "customer_id");
                    table.ForeignKey(
                        name: "test_appointment_dealer_id_fkey",
                        column: x => x.dealer_id,
                        principalTable: "dealer",
                        principalColumn: "dealer_id");
                    table.ForeignKey(
                        name: "test_appointment_vehicle_id_fkey",
                        column: x => x.vehicle_id,
                        principalTable: "vehicle",
                        principalColumn: "vehicle_id");
                });

            migrationBuilder.CreateTable(
                name: "quote",
                columns: table => new
                {
                    quote_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    vehicle_id = table.Column<int>(type: "integer", nullable: false),
                    dealer_id = table.Column<int>(type: "integer", nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric", nullable: false),
                    quote_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    PromotionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("quote_pkey", x => x.quote_id);
                    table.ForeignKey(
                        name: "FK_quote_promotion_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "promotion",
                        principalColumn: "promotion_id");
                    table.ForeignKey(
                        name: "quote_customer_id_fkey",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "customer_id");
                    table.ForeignKey(
                        name: "quote_dealer_id_fkey",
                        column: x => x.dealer_id,
                        principalTable: "dealer",
                        principalColumn: "dealer_id");
                    table.ForeignKey(
                        name: "quote_vehicle_id_fkey",
                        column: x => x.vehicle_id,
                        principalTable: "vehicle",
                        principalColumn: "vehicle_id");
                });

            migrationBuilder.CreateTable(
                name: "feedback",
                columns: table => new
                {
                    feedback_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    vehicle_id = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: true),
                    comment = table.Column<string>(type: "text", nullable: true),
                    create_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("feedback_pkey", x => x.feedback_id);
                    table.ForeignKey(
                        name: "feedback_customer_id_fkey",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "customer_id");
                    table.ForeignKey(
                        name: "feedback_order_id_fkey",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "feedback_vehicle_id_fkey",
                        column: x => x.vehicle_id,
                        principalTable: "vehicle",
                        principalColumn: "vehicle_id");
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    payment_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    method = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    amount = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: true),
                    transaction_id = table.Column<string>(type: "text", nullable: true),
                    checkout_url = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("payment_pkey", x => x.payment_id);
                    table.ForeignKey(
                        name: "payment_customer_id_fkey",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "customer_id");
                    table.ForeignKey(
                        name: "payment_order_id_fkey",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_agreement_customer_id",
                table: "agreement",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_agreement_store_id",
                table: "agreement",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "customer_email_key",
                table: "customer",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "dealer_email_key",
                table: "dealer",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_dealer_store_id",
                table: "dealer",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_customer_id",
                table: "feedback",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_order_id",
                table: "feedback",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_vehicle_id",
                table: "feedback",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_customer_id",
                table: "orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_dealer_id",
                table: "orders",
                column: "dealer_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_store_id",
                table: "orders",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_customer_id",
                table: "payment",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_order_id",
                table: "payment",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_promotion_store_id",
                table: "promotion",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_quote_customer_id",
                table: "quote",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_quote_dealer_id",
                table: "quote",
                column: "dealer_id");

            migrationBuilder.CreateIndex(
                name: "IX_quote_PromotionId",
                table: "quote",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_quote_vehicle_id",
                table: "quote",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_brand_id",
                table: "staff",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "staff_email_key",
                table: "staff",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_storage_store_id",
                table: "storage",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_storage_vehicle_id",
                table: "storage",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "store_email_key",
                table: "store",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_customer_customer_id",
                table: "store_customer",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_test_appointment_customer_id",
                table: "test_appointment",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_test_appointment_dealer_id",
                table: "test_appointment",
                column: "dealer_id");

            migrationBuilder.CreateIndex(
                name: "IX_test_appointment_vehicle_id",
                table: "test_appointment",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_brand_id",
                table: "vehicle",
                column: "brand_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agreement");

            migrationBuilder.DropTable(
                name: "feedback");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "quote");

            migrationBuilder.DropTable(
                name: "staff");

            migrationBuilder.DropTable(
                name: "storage");

            migrationBuilder.DropTable(
                name: "store_customer");

            migrationBuilder.DropTable(
                name: "test_appointment");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "promotion");

            migrationBuilder.DropTable(
                name: "vehicle");

            migrationBuilder.DropTable(
                name: "customer");

            migrationBuilder.DropTable(
                name: "dealer");

            migrationBuilder.DropTable(
                name: "brand");

            migrationBuilder.DropTable(
                name: "store");
        }
    }
}
