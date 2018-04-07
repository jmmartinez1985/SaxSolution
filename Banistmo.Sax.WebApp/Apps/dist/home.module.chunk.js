webpackJsonp(["home.module"],{

/***/ "./src/app/pages/home/home.component.html":
/***/ (function(module, exports) {

module.exports = "<section class=\"content\">\r\n    <header class=\"content__title\">\r\n        <h1>Dashboard</h1>\r\n        <small>Welcome to the unique Material Design admin web app experience!</small>\r\n\r\n        <div class=\"actions\">\r\n            <a class=\"actions__item zmdi zmdi-trending-up\" [routerLink]=\"['/home']\"></a>\r\n            <a class=\"actions__item zmdi zmdi-check-all\" [routerLink]=\"['/home']\"></a>\r\n            <div dropdown>\r\n                <i dropdownToggle class=\"actions__item zmdi zmdi-more-vert\"></i>\r\n                <div *dropdownMenu class=\"dropdown-menu dropdown-menu-right\">\r\n                    <a [routerLink]=\"['/home']\" class=\"dropdown-item\">Refresh</a>\r\n                    <a [routerLink]=\"['/home']\" class=\"dropdown-item\">Manage Widgets</a>\r\n                    <a [routerLink]=\"['/home']\" class=\"dropdown-item\">Settings</a>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </header>\r\n\r\n    <quick-stats></quick-stats>\r\n    <div class=\"row\">\r\n        <div class=\"col-md-6\">\r\n            <div class=\"card\">\r\n                <div class=\"card-body\">\r\n                    <h4 class=\"card-title\">Sales Statistics</h4>\r\n                    <h6 class=\"card-subtitle\">Vestibulum purus quam scelerisque, mollis nonummy metus</h6>\r\n\r\n                    <div flot [options]=\"salesStatChartOptions\" [dataset]=\"salesStatChartData\" [height]=\"200\"></div>\r\n                    <div class=\"flot-chart-legends flot-chart-legends--curved\"></div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"col-md-6\">\r\n            <div class=\"card\">\r\n                <div class=\"card-body\">\r\n                    <h4 class=\"card-title\">Growth Rate</h4>\r\n                    <h6 class=\"card-subtitle\">Commodo luctus nisi erat porttitor ligula eget lacinia odio semnec elit</h6>\r\n\r\n                    <div flot [options]=\"lineChartOptions\" [dataset]=\"lineChartData\" [height]=\"200\"></div>\r\n                    <div class=\"flot-chart-legends flot-chart-legends--lines\"></div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n\r\n    <div class=\"widget-lists card-columns\">\r\n        <widget-past-days></widget-past-days>\r\n\r\n        <widget-random-post></widget-random-post>\r\n\r\n        <widget-visitors></widget-visitors>\r\n\r\n        <widget-pie-charts></widget-pie-charts>\r\n\r\n        <widget-recent-posts></widget-recent-posts>\r\n\r\n        <widget-todo-lists></widget-todo-lists>\r\n    </div>\r\n</section>"

/***/ }),

/***/ "./src/app/pages/home/home.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return HomeComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var HomeComponent = /** @class */ (function () {
    function HomeComponent() {
        // Sales Chart
        this.salesStatChartOptions = {
            series: {
                shadowSize: 0,
                curvedLines: {
                    apply: true,
                    active: true,
                    monotonicFit: true
                },
                points: {
                    show: false
                }
            },
            grid: {
                borderWidth: 1,
                borderColor: '#edf9fc',
                show: true,
                hoverable: true,
                clickable: true
            },
            xaxis: {
                tickColor: '#edf9fc',
                tickDecimals: 0,
                font: {
                    lineHeight: 13,
                    style: 'normal',
                    color: '#999999',
                    size: 11
                }
            },
            yaxis: {
                tickColor: '#edf9fc',
                font: {
                    lineHeight: 13,
                    style: 'normal',
                    color: '#999999',
                    size: 11,
                },
                min: +5
            },
            legend: {
                container: '.flot-chart-legends--curved',
                backgroundOpacity: 0.5,
                noColumns: 0,
                backgroundColor: '#fff',
                lineWidth: 0,
                labelBoxBorderColor: '#fff'
            }
        };
        this.salesStatChartData = [
            {
                label: 'Grey',
                color: '#f6f6f6',
                lines: {
                    show: true,
                    lineWidth: 0,
                    fill: 1,
                    fillColor: {
                        colors: ['rgba(246,246,246,0.1)', '#f6f6f6']
                    }
                },
                data: [[10, 90], [20, 40], [30, 80], [40, 20], [50, 90], [60, 20], [70, 60]],
            },
            {
                label: 'Cyan',
                color: '#00BCD4',
                lines: {
                    show: true,
                    lineWidth: 0.1,
                    fill: 1,
                    fillColor: {
                        colors: ['rgba(0,188,212,0.001)', '#00BCD4']
                    }
                },
                data: [[10, 80], [20, 30], [30, 70], [40, 10], [50, 80], [60, 10], [70, 50]]
            }
        ];
        // Line Chart
        this.lineChartOptions = {
            series: {
                lines: {
                    show: true,
                    barWidth: 0.05,
                    fill: 0
                }
            },
            shadowSize: 0.1,
            grid: {
                borderWidth: 1,
                borderColor: '#edf9fc',
                show: true,
                hoverable: true,
                clickable: true
            },
            yaxis: {
                tickColor: '#edf9fc',
                tickDecimals: 0,
                font: {
                    lineHeight: 13,
                    style: 'normal',
                    color: '#9f9f9f',
                },
                shadowSize: 0
            },
            xaxis: {
                tickColor: '#fff',
                tickDecimals: 0,
                font: {
                    lineHeight: 13,
                    style: 'normal',
                    color: '#9f9f9f'
                },
                shadowSize: 0,
            },
            legend: {
                container: '.flot-chart-legends--lines',
                backgroundOpacity: 0.5,
                noColumns: 0,
                backgroundColor: '#fff',
                lineWidth: 0,
                labelBoxBorderColor: '#fff'
            }
        };
        this.lineChartData = [
            {
                label: 'Green',
                data: [[1, 60], [2, 30], [3, 50], [4, 100], [5, 10], [6, 90], [7, 85]],
                color: '#32c787'
            },
            {
                label: 'Blue',
                data: [[1, 20], [2, 90], [3, 60], [4, 40], [5, 100], [6, 25], [7, 65]],
                color: '#03A9F4'
            },
            {
                label: 'Amber',
                data: [[1, 100], [2, 20], [3, 60], [4, 90], [5, 80], [6, 10], [7, 5]],
                color: '#f5c942'
            }
        ];
    }
    HomeComponent.prototype.ngOnInit = function () {
    };
    HomeComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'app-home',
            template: __webpack_require__("./src/app/pages/home/home.component.html")
        }),
        __metadata("design:paramtypes", [])
    ], HomeComponent);
    return HomeComponent;
}());



/***/ }),

/***/ "./src/app/pages/home/home.module.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "HomeModule", function() { return HomeModule; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_common__ = __webpack_require__("./node_modules/@angular/common/esm5/common.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_router__ = __webpack_require__("./node_modules/@angular/router/esm5/router.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_ngx_bootstrap_dropdown__ = __webpack_require__("./node_modules/ngx-bootstrap/dropdown/index.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__shared_shared_module__ = __webpack_require__("./src/app/shared/shared.module.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__home_component__ = __webpack_require__("./src/app/pages/home/home.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__quick_stats_quick_stats_component__ = __webpack_require__("./src/app/pages/home/quick-stats/quick-stats.component.ts");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};







var HOME_ROUTE = [
    { path: '', component: __WEBPACK_IMPORTED_MODULE_5__home_component__["a" /* HomeComponent */] }
];
var HomeModule = /** @class */ (function () {
    function HomeModule() {
    }
    HomeModule = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_1__angular_core__["NgModule"])({
            declarations: [
                __WEBPACK_IMPORTED_MODULE_5__home_component__["a" /* HomeComponent */],
                __WEBPACK_IMPORTED_MODULE_6__quick_stats_quick_stats_component__["a" /* QuickStatsComponent */]
            ],
            imports: [
                __WEBPACK_IMPORTED_MODULE_0__angular_common__["CommonModule"],
                __WEBPACK_IMPORTED_MODULE_4__shared_shared_module__["a" /* SharedModule */],
                __WEBPACK_IMPORTED_MODULE_3_ngx_bootstrap_dropdown__["a" /* BsDropdownModule */].forRoot(),
                __WEBPACK_IMPORTED_MODULE_2__angular_router__["c" /* RouterModule */].forChild(HOME_ROUTE)
            ]
        })
    ], HomeModule);
    return HomeModule;
}());



/***/ }),

/***/ "./src/app/pages/home/quick-stats/quick-stats.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"row quick-stats\">\r\n  <div *ngFor=\"let quickStatData of quickStatChartData\" class=\"col-sm-6 col-md-3\">\r\n    <div class=\"quick-stats__item bg-{{ quickStatData.color }}\">\r\n      <div class=\"quick-stats__info\">\r\n        <h2>{{ quickStatData.value }}</h2>\r\n        <small>{{ quickStatData.title }}</small>\r\n      </div>\r\n\r\n      <sparkline class=\"quick-stats__chart\" [dataset]=\"quickStatData.data\" [options]=\"quickStatChartOptions\"></sparkline>\r\n    </div>\r\n  </div>\r\n</div>\r\n"

/***/ }),

/***/ "./src/app/pages/home/quick-stats/quick-stats.component.scss":
/***/ (function(module, exports) {

module.exports = ".quick-stats__item {\n  padding: 1.5rem 1.5rem 1.45rem;\n  border-radius: 2px;\n  -webkit-box-shadow: 0 2px 5px rgba(0, 0, 0, 0.08);\n          box-shadow: 0 2px 5px rgba(0, 0, 0, 0.08);\n  margin-bottom: 30px;\n  display: -webkit-box;\n  display: -ms-flexbox;\n  display: flex;\n  -webkit-box-align: baseline;\n      -ms-flex-align: baseline;\n          align-items: baseline; }\n  .quick-stats__item::after {\n    display: block;\n    clear: both;\n    content: \"\"; }\n  .quick-stats__chart,\n.quick-stats__info {\n  display: inline-block;\n  vertical-align: middle; }\n  .quick-stats__info {\n  min-width: 0; }\n  .quick-stats__info > h2,\n  .quick-stats__info > small {\n    line-height: 100%;\n    overflow: hidden;\n    text-overflow: ellipsis;\n    white-space: nowrap; }\n  .quick-stats__info > h2 {\n    font-weight: normal;\n    margin: 0;\n    font-size: 1.6rem;\n    color: #FFFFFF; }\n  .quick-stats__info > small {\n    font-size: 1rem;\n    display: block;\n    color: rgba(255, 255, 255, 0.8);\n    margin-top: 0.6rem; }\n  .quick-stats__chart {\n  margin-left: auto;\n  padding-left: 1.2rem; }\n  @media (min-width: 576px) and (max-width: 1199px) {\n    .quick-stats__chart {\n      display: none; } }\n  .stats {\n  padding-top: 1rem; }\n  .stats__item {\n  background-color: #FFFFFF;\n  border-radius: 2px;\n  -webkit-box-shadow: 0 1px 2px rgba(0, 0, 0, 0.075);\n          box-shadow: 0 1px 2px rgba(0, 0, 0, 0.075);\n  margin-bottom: 30px;\n  padding: 1rem; }\n  .stats__chart {\n  border-radius: 2px;\n  padding-top: 2rem;\n  margin-top: -2rem;\n  -webkit-box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);\n          box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);\n  overflow: hidden; }\n  .stats__chart .flot-chart {\n    margin: 0 -12px -12px; }\n  .stats__info {\n  padding: 1.8rem 1rem 0.5rem;\n  position: relative;\n  text-align: center; }\n  .stats__info h2 {\n    font-size: 1.5rem;\n    margin: 0; }\n  .stats__info small {\n    display: block;\n    font-size: 1rem;\n    margin-top: 0.4rem;\n    color: #9c9c9c; }\n"

/***/ }),

/***/ "./src/app/pages/home/quick-stats/quick-stats.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return QuickStatsComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var QuickStatsComponent = /** @class */ (function () {
    function QuickStatsComponent() {
        this.quickStatChartData = [
            {
                title: 'Website Traffics',
                value: '987,459',
                data: [6, 4, 8, 6, 5, 6, 7, 8, 3, 5, 9, 5],
                color: 'light-blue'
            },
            {
                title: 'Website Impressions',
                value: '356,785K',
                data: [4, 7, 6, 2, 5, 3, 8, 6, 6, 4, 8, 6],
                color: 'amber'
            },
            {
                title: 'Total Sales',
                value: '$ 458,778',
                data: [9, 4, 6, 5, 6, 4, 5, 7, 9, 3, 6, 5],
                color: 'purple'
            },
            {
                title: 'Support Tickets',
                value: '201',
                data: [5, 6, 3, 9, 7, 5, 4, 6, 5, 6, 4, 9],
                color: 'red'
            }
        ];
        this.quickStatChartOptions = {
            type: 'bar',
            height: '36px',
            barWidth: 3,
            barColor: 'rgba(255,255,255,0.8)',
            barSpacing: 2
        };
    }
    QuickStatsComponent.prototype.ngOnInit = function () {
    };
    QuickStatsComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'quick-stats',
            template: __webpack_require__("./src/app/pages/home/quick-stats/quick-stats.component.html"),
            styles: [__webpack_require__("./src/app/pages/home/quick-stats/quick-stats.component.scss")]
        }),
        __metadata("design:paramtypes", [])
    ], QuickStatsComponent);
    return QuickStatsComponent;
}());



/***/ }),

/***/ "./src/app/shared/components/widgets/contacts/contacts.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"card widget-contacts\">\r\n  <div class=\"card-body\">\r\n    <h4 class=\"card-title\">Contact Information</h4>\r\n    <h6 class=\"card-subtitle\">Fusce eget dolor id justo luctus commodo vel pharetra nisi</h6>\r\n\r\n    <ul class=\"icon-list\">\r\n      <li><i class=\"zmdi zmdi-phone\"></i> {{ contactData.phone }}</li>\r\n      <li><i class=\"zmdi zmdi-email\"></i> {{ contactData.email }}</li>\r\n      <li><i class=\"zmdi zmdi-facebook-box\"></i> {{ contactData.facebook }}</li>\r\n      <li><i class=\"zmdi zmdi-twitter\"></i> {{ contactData.twitter }}</li>\r\n      <li><i class=\"zmdi zmdi-pin\"></i>\r\n        <address>\r\n          {{ contactData.address }}\r\n        </address>\r\n      </li>\r\n    </ul>\r\n  </div>\r\n\r\n  <a class=\"widget-contacts__map\" href=\"\">\r\n    <img [src]=\"contactData.map\" alt=\"\">\r\n  </a>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/components/widgets/contacts/contacts.component.scss":
/***/ (function(module, exports) {

module.exports = ".widget-contacts__map {\n  display: block;\n  padding: 3px; }\n  .widget-contacts__map img {\n    width: 100%;\n    border-radius: 2px;\n    margin: -10px 0 -1px; }\n"

/***/ }),

/***/ "./src/app/shared/components/widgets/contacts/contacts.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ContactsComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var ContactsComponent = /** @class */ (function () {
    function ContactsComponent() {
        this.contactData = {
            phone: '00971123456789',
            email: 'malinda.h@gmail.com',
            facebook: 'malinda.hollaway',
            twitter: '@malinda (twitter.com/malinda)',
            address: '44-46 Morningside Road,  Edinburgh, Scotland',
            map: './assets/demo/img/widgets/map.png'
        };
    }
    ContactsComponent.prototype.ngOnInit = function () {
    };
    ContactsComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'widget-contacts',
            template: __webpack_require__("./src/app/shared/components/widgets/contacts/contacts.component.html"),
            styles: [__webpack_require__("./src/app/shared/components/widgets/contacts/contacts.component.scss")]
        }),
        __metadata("design:paramtypes", [])
    ], ContactsComponent);
    return ContactsComponent;
}());



/***/ }),

/***/ "./src/app/shared/components/widgets/past-days/past-days.component.html":
/***/ (function(module, exports) {

module.exports = "<figure class=\"card card--inverse widget-past-days\">\r\n  <div class=\"card-body\">\r\n    <h4 class=\"card-title\">For the past 30 days</h4>\r\n    <h6 class=\"card-subtitle\">Pellentesque ornare sem lacinia quam</h6>\r\n  </div>\r\n\r\n  <div class=\"widget-past-days__main\">\r\n    <div flot [options]=\"pastDaysChartOptions\" [dataset]=\"pastDaysChartData\" [height]=\"100\"></div>\r\n  </div>\r\n\r\n  <div class=\"listview listview--inverse listview--striped\">\r\n    <div *ngFor=\"let pastDaysData of pastDaysData\" class=\"listview__item\">\r\n      <div class=\"widget-past-days__info\">\r\n        <small>{{ pastDaysData.title }}</small>\r\n        <h3>{{ pastDaysData.value }}</h3>\r\n      </div>\r\n\r\n      <div class=\"widget-past-days__chart hidden-sm\">\r\n        <sparkline [dataset]=\"pastDaysData.chartData\" [options]=\"pastDaysSubChartDataOptions\"></sparkline>\r\n      </div>\r\n    </div>\r\n  </div>\r\n</figure>"

/***/ }),

/***/ "./src/app/shared/components/widgets/past-days/past-days.component.scss":
/***/ (function(module, exports) {

module.exports = ".widget-past-days {\n  background-color: #39bbb0;\n  overflow: hidden; }\n\n.widget-past-days__main {\n  margin: 0 -10px; }\n\n.widget-past-days__chart {\n  opacity: 0.75;\n  margin: 0.55rem 0 0 auto; }\n\n.widget-past-days__info small {\n  font-size: 1rem;\n  color: rgba(255, 255, 255, 0.9); }\n\n.widget-past-days__info h3 {\n  margin: 0;\n  color: #FFFFFF;\n  font-weight: normal; }\n"

/***/ }),

/***/ "./src/app/shared/components/widgets/past-days/past-days.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return PastDaysComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var PastDaysComponent = /** @class */ (function () {
    function PastDaysComponent() {
        this.pastDaysChartOptions = {
            series: {
                shadowSize: 0,
                curvedLines: {
                    apply: true,
                    active: true,
                    monotonicFit: true
                },
                lines: {
                    show: false,
                    lineWidth: 0
                }
            },
            grid: {
                borderWidth: 0,
                labelMargin: 10,
                hoverable: true,
                clickable: true,
                mouseActiveRadius: 6
            },
            xaxis: {
                tickDecimals: 0,
                ticks: false
            },
            yaxis: {
                tickDecimals: 0,
                ticks: false
            },
            legend: {
                show: false
            }
        };
        this.pastDaysChartData = [{
                label: 'Product 1',
                stack: true,
                color: '#35424b',
                lines: {
                    show: true,
                    fill: 1,
                    fillColor: {
                        colors: ['rgba(255,255,255,0)', 'rgba(255,255,255,0.6)']
                    }
                },
                data: [[1, 3], [2, 9], [3, 8], [4, 6], [5, 11], [6, 4], [7, 7]]
            }];
        this.pastDaysData = [
            {
                title: 'Page Views',
                value: '47,896,536',
                chartData: [6, 9, 5, 6, 3, 7, 5, 4, 6, 5, 6, 4, 2, 5, 8, 2, 6, 9]
            },
            {
                title: 'Site Visitors',
                value: '24,456,799',
                chartData: [5, 7, 2, 5, 2, 8, 6, 7, 6, 5, 3, 1, 9, 3, 5, 8, 2, 4]
            },
            {
                title: 'Total Clicks',
                value: '13,965',
                chartData: [5, 7, 2, 5, 2, 8, 6, 7, 6, 5, 3, 1, 9, 3, 5, 8, 2, 4]
            },
            {
                title: 'Total Returns',
                value: '198',
                chartData: [3, 9, 1, 3, 5, 6, 7, 6, 8, 2, 5, 2, 7, 5, 6, 7, 6, 8]
            },
        ];
        this.pastDaysSubChartDataOptions = {
            type: 'bar',
            height: '36px',
            barWidth: 3,
            barColor: '#fff',
            barSpacing: 2
        };
    }
    PastDaysComponent.prototype.ngOnInit = function () {
    };
    PastDaysComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'widget-past-days',
            template: __webpack_require__("./src/app/shared/components/widgets/past-days/past-days.component.html"),
            styles: [__webpack_require__("./src/app/shared/components/widgets/past-days/past-days.component.scss")]
        }),
        __metadata("design:paramtypes", [])
    ], PastDaysComponent);
    return PastDaysComponent;
}());



/***/ }),

/***/ "./src/app/shared/components/widgets/photos/photos.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"card widget-pictures\">\r\n  <div class=\"card-header\">\r\n    <h2 class=\"card-title\">Augue laoreet rutrum</h2>\r\n    <small class=\"card-subtitle\">Cras congue nec lorem eget posuere</small>\r\n  </div>\r\n\r\n  <div class=\"widget-pictures__body row\">\r\n    <a class=\"col-4\" *ngFor=\"let pictureData of picturesData\" href=\"\">\r\n      <img [src]=\"pictureData\" alt=\"\">\r\n    </a>\r\n  </div>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/components/widgets/photos/photos.component.scss":
/***/ (function(module, exports) {

module.exports = ".widget-pictures__body {\n  margin: 0;\n  padding: 2px;\n  text-align: center; }\n  .widget-pictures__body::after {\n    display: block;\n    clear: both;\n    content: \"\"; }\n  .widget-pictures__body > a {\n    padding: 2px;\n    display: block; }\n  .widget-pictures__body > a img {\n      width: 100%;\n      border-radius: 2px; }\n  .widget-pictures__body > a:hover {\n      opacity: 0.9; }\n"

/***/ }),

/***/ "./src/app/shared/components/widgets/photos/photos.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return PhotosComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var PhotosComponent = /** @class */ (function () {
    function PhotosComponent() {
        this.picturesData = [
            './assets/demo/img/widgets/headers/1.png',
            './assets/demo/img/widgets/headers/2.png',
            './assets/demo/img/widgets/headers/3.png',
            './assets/demo/img/widgets/headers/4.png',
            './assets/demo/img/widgets/headers/5.png',
            './assets/demo/img/widgets/headers/6.png',
            './assets/demo/img/widgets/headers/7.png',
            './assets/demo/img/widgets/headers/8.png',
            './assets/demo/img/widgets/headers/9.png'
        ];
    }
    PhotosComponent.prototype.ngOnInit = function () {
    };
    PhotosComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'widget-photos',
            template: __webpack_require__("./src/app/shared/components/widgets/photos/photos.component.html"),
            styles: [__webpack_require__("./src/app/shared/components/widgets/photos/photos.component.scss")]
        }),
        __metadata("design:paramtypes", [])
    ], PhotosComponent);
    return PhotosComponent;
}());



/***/ }),

/***/ "./src/app/shared/components/widgets/pie-charts/pie-charts.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"card card-inverse widget-pie\">\r\n\r\n  <div class=\"widget-pie__item\">\r\n    <div EasyPieChart class=\"easy-pie-chart\" [size]=\"80\" data-percent=\"92\">\r\n      <span class=\"easy-pie-chart__value\">92</span>\r\n    </div>\r\n    <div class=\"widget-pie__title\">Email<br> Scheduled</div>\r\n  </div>\r\n\r\n  <div class=\"widget-pie__item\">\r\n    <div EasyPieChart class=\"easy-pie-chart\" [size]=\"80\" data-percent=\"23\">\r\n      <span class=\"easy-pie-chart__value\">23</span>\r\n    </div>\r\n    <div class=\"widget-pie__title\">Email<br> Bounced</div>\r\n  </div>\r\n\r\n  <div class=\"widget-pie__item\">\r\n    <div EasyPieChart class=\"easy-pie-chart\" [size]=\"80\" data-percent=\"52\">\r\n      <span class=\"easy-pie-chart__value\">52</span>\r\n    </div>\r\n    <div class=\"widget-pie__title\">Email<br> Opened</div>\r\n  </div>\r\n\r\n  <div class=\"widget-pie__item\">\r\n    <div EasyPieChart class=\"easy-pie-chart\" [size]=\"80\" data-percent=\"44\">\r\n      <span class=\"easy-pie-chart__value\">44</span>\r\n    </div>\r\n    <div class=\"widget-pie__title\">Storage<br>Remaining</div>\r\n  </div>\r\n\r\n  <div class=\"widget-pie__item\">\r\n    <div EasyPieChart class=\"easy-pie-chart\" [size]=\"80\" data-percent=\"78\">\r\n      <span class=\"easy-pie-chart__value\">78</span>\r\n    </div>\r\n    <div class=\"widget-pie__title\">Web Page<br> Views</div>\r\n  </div>\r\n\r\n  <div class=\"widget-pie__item\">\r\n    <div EasyPieChart class=\"easy-pie-chart\" [size]=\"80\" data-percent=\"32\">\r\n      <span class=\"easy-pie-chart__value\">32</span>\r\n    </div>\r\n    <div class=\"widget-pie__title\">Server<br> Processing</div>\r\n  </div>\r\n\r\n</div>"

/***/ }),

/***/ "./src/app/shared/components/widgets/pie-charts/pie-charts.component.scss":
/***/ (function(module, exports) {

module.exports = ".widget-pie {\n  background-color: #ed1c29;\n  -webkit-box-orient: horizontal;\n  -webkit-box-direction: normal;\n      -ms-flex-direction: row;\n          flex-direction: row;\n  -ms-flex-wrap: wrap;\n      flex-wrap: wrap; }\n  .widget-pie::after {\n    display: block;\n    clear: both;\n    content: \"\"; }\n  .widget-pie__item {\n  width: 33.33333%;\n  float: left;\n  text-align: center;\n  padding: 20px 5px; }\n  .widget-pie__item:nth-child(2n) {\n    background-color: rgba(255, 255, 255, 0.1); }\n  .widget-pie__title {\n  color: #FFFFFF; }\n"

/***/ }),

/***/ "./src/app/shared/components/widgets/pie-charts/pie-charts.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return PieChartsComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var PieChartsComponent = /** @class */ (function () {
    function PieChartsComponent() {
    }
    PieChartsComponent.prototype.ngOnInit = function () {
    };
    PieChartsComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'widget-pie-charts',
            template: __webpack_require__("./src/app/shared/components/widgets/pie-charts/pie-charts.component.html"),
            styles: [__webpack_require__("./src/app/shared/components/widgets/pie-charts/pie-charts.component.scss")]
        }),
        __metadata("design:paramtypes", [])
    ], PieChartsComponent);
    return PieChartsComponent;
}());



/***/ }),

/***/ "./src/app/shared/components/widgets/profile/profile.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"card card--inverse widget-profile\">\r\n  <div class=\"card-body\">\r\n    <div class=\"text-center\">\r\n      <img src=\"./assets/demo/img/profile-pics/profile-pic.jpg\" class=\"widget-profile__img\" alt=\"\">\r\n\r\n      <h4 class=\"card-title\">Malinda Hollway</h4>\r\n    </div>\r\n\r\n    <div class=\"actions actions--inverse\">\r\n      <div dropdown class=\"actions__item\">\r\n        <i dropdownToggle class=\"zmdi zmdi-more-vert\"></i>\r\n        <div *dropdownMenu class=\"dropdown-menu dropdown-menu-right\">\r\n          <a [routerLink]=\"['/widgets']\" class=\"dropdown-item\">Refresh</a>\r\n          <a [routerLink]=\"['/widgets']\" class=\"dropdown-item\">Manage Widgets</a>\r\n          <a [routerLink]=\"['/widgets']\" class=\"dropdown-item\">Settings</a>\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </div>\r\n\r\n  <div class=\"widget-profile__list\">\r\n    <div class=\"media\">\r\n      <div class=\"media-left\">\r\n        <div class=\"avatar-char\"><i class=\"zmdi zmdi-phone\"></i></div>\r\n      </div>\r\n      <div class=\"media-body\">\r\n        <strong>011 55694785</strong>\r\n        <small>Home</small>\r\n      </div>\r\n    </div>\r\n    <div class=\"media\">\r\n      <div class=\"media-left\">\r\n        <div class=\"avatar-char\"><i class=\"zmdi zmdi-email\"></i></div>\r\n      </div>\r\n      <div class=\"media-body\">\r\n        <strong>m-hollaway@gmail.com</strong>\r\n        <small>Email</small>\r\n      </div>\r\n    </div>\r\n    <div class=\"media\">\r\n      <div class=\"media-left\">\r\n        <div class=\"avatar-char\"><i class=\"zmdi zmdi-twitter\"></i></div>\r\n      </div>\r\n      <div class=\"media-body\">\r\n        <strong>@m-hollaway</strong>\r\n        <small>Twitter</small>\r\n      </div>\r\n    </div>\r\n    <div class=\"media\">\r\n      <div class=\"media-left\">\r\n        <div class=\"avatar-char\"><i class=\"zmdi zmdi-facebook\"></i></div>\r\n      </div>\r\n      <div class=\"media-body\">\r\n        <strong>facebook/hollaway</strong>\r\n        <small>Facebook</small>\r\n      </div>\r\n    </div>\r\n    <div class=\"media\">\r\n      <div class=\"media-left\">\r\n        <div class=\"avatar-char\"><i class=\"zmdi zmdi-github\"></i></div>\r\n      </div>\r\n      <div class=\"media-body\">\r\n        <strong>github.com/hollaway</strong>\r\n        <small>Github</small>\r\n      </div>\r\n\r\n      <br>\r\n    </div>\r\n  </div>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/components/widgets/profile/profile.component.scss":
/***/ (function(module, exports) {

module.exports = ".widget-profile {\n  background-color: #03A9F4; }\n  .widget-profile .card-header {\n    background-color: rgba(255, 255, 255, 0.1);\n    text-align: center; }\n  .widget-profile .card-header .btn--float {\n      color: #03A9F4; }\n  .widget-profile .avatar-char {\n    background-color: rgba(255, 255, 255, 0.95);\n    color: #03A9F4;\n    margin-right: 1.2rem; }\n  .widget-profile__img {\n  width: 100px;\n  height: 100px;\n  border-radius: 50%;\n  margin-bottom: 1.2rem;\n  border: 5px solid rgba(255, 255, 255, 0.1); }\n  .widget-profile__list {\n  color: #FFFFFF; }\n  .widget-profile__list .media {\n    padding: 1rem 2rem; }\n  .widget-profile__list .media:nth-child(even) {\n      background-color: rgba(255, 255, 255, 0.1); }\n  .widget-profile__list .media-body strong {\n    display: block;\n    font-weight: 500; }\n  .widget-profile__list .media-body small {\n    color: rgba(255, 255, 255, 0.8);\n    font-size: 0.92rem; }\n"

/***/ }),

/***/ "./src/app/shared/components/widgets/profile/profile.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ProfileComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var ProfileComponent = /** @class */ (function () {
    function ProfileComponent() {
    }
    ProfileComponent.prototype.ngOnInit = function () {
    };
    ProfileComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'widget-profile',
            template: __webpack_require__("./src/app/shared/components/widgets/profile/profile.component.html"),
            styles: [__webpack_require__("./src/app/shared/components/widgets/profile/profile.component.scss")]
        }),
        __metadata("design:paramtypes", [])
    ], ProfileComponent);
    return ProfileComponent;
}());



/***/ }),

/***/ "./src/app/shared/components/widgets/random-post/random-post.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"card\">\r\n  <img class=\"card-img-top\" src=\"./assets/demo/img/widgets/note.png\" alt=\"Card image cap\">\r\n\r\n  <div class=\"card-body\">\r\n    <h4 class=\"card-title\">Pellentesque Ligula Fringilla</h4>\r\n    <h6 class=\"card-subtitle\">by Malinda Hollaway on 19th June 2015 at 09:10 AM</h6>\r\n\r\n    <p>Donec ullamcorper nulla non metus auctor fringilla. Cras justo odio, dapibus ac facilisis in, egestas eget quam. Fusce dapibus, tellus ac cursus commodo, tortor mauris condimentum nibh, ut fermentum massa justo sit amet risus. Vestibulum id ligula porta felis euismod semper. Nulla vitae elit libero, a pharetra.</p>\r\n    <a href=\"\" class=\"view-more text-left\">View Article...</a>\r\n  </div>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/components/widgets/random-post/random-post.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return RandomPostComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var RandomPostComponent = /** @class */ (function () {
    function RandomPostComponent() {
    }
    RandomPostComponent.prototype.ngOnInit = function () {
    };
    RandomPostComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'widget-random-post',
            template: __webpack_require__("./src/app/shared/components/widgets/random-post/random-post.component.html")
        }),
        __metadata("design:paramtypes", [])
    ], RandomPostComponent);
    return RandomPostComponent;
}());



/***/ }),

/***/ "./src/app/shared/components/widgets/ratings/ratings.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"card widget-ratings\">\r\n  <div class=\"card-body text-center\">\r\n    <h4 class=\"card-title\">Average Rating 3.0</h4>\r\n\r\n    <div class=\"widget-ratings__star\">\r\n      <i class=\"zmdi zmdi-star active\"></i>\r\n      <i class=\"zmdi zmdi-star active\"></i>\r\n      <i class=\"zmdi zmdi-star active\"></i>\r\n      <i class=\"zmdi zmdi-star\"></i>\r\n      <i class=\"zmdi zmdi-star\"></i>\r\n    </div>\r\n\r\n    <div class=\"widget-ratings__item\">\r\n      <div class=\"float-left\">1 <i class=\"zmdi zmdi-star\"></i></div>\r\n      <div class=\"float-right\">20</div>\r\n\r\n      <div class=\"widget-ratings__progress\">\r\n        <div class=\"progress bg-wa\">\r\n          <div class=\"progress-bar bg-warning\" style=\"width: 20%\" aria-valuenow=\"20\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>\r\n        </div>\r\n      </div>\r\n    </div>\r\n\r\n    <div class=\"widget-ratings__item\">\r\n      <div class=\"float-left\">2 <i class=\"zmdi zmdi-star\"></i></div>\r\n      <div class=\"float-right\">45</div>\r\n\r\n      <div class=\"widget-ratings__progress\">\r\n        <div class=\"progress\">\r\n          <div class=\"progress-bar bg-warning\" style=\"width: 45%\" aria-valuenow=\"45\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>\r\n        </div>\r\n      </div>\r\n    </div>\r\n\r\n    <div class=\"widget-ratings__item\">\r\n      <div class=\"float-left\">3 <i class=\"zmdi zmdi-star\"></i></div>\r\n      <div class=\"float-right\">60</div>\r\n\r\n      <div class=\"widget-ratings__progress\">\r\n        <div class=\"progress\">\r\n          <div class=\"progress-bar bg-warning\" style=\"width: 60%\" aria-valuenow=\"60\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>\r\n        </div>\r\n      </div>\r\n    </div>\r\n\r\n    <div class=\"widget-ratings__item\">\r\n      <div class=\"float-left\">4 <i class=\"zmdi zmdi-star\"></i></div>\r\n      <div class=\"float-right\">78</div>\r\n\r\n      <div class=\"widget-ratings__progress\">\r\n        <div class=\"progress\">\r\n          <div class=\"progress-bar bg-warning\" style=\"width: 78%\" aria-valuenow=\"78\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>\r\n        </div>\r\n      </div>\r\n    </div>\r\n\r\n    <div class=\"widget-ratings__item\">\r\n      <div class=\"float-left\">5 <i class=\"zmdi zmdi-star\"></i></div>\r\n      <div class=\"float-right\">52</div>\r\n\r\n      <div class=\"widget-ratings__progress\">\r\n        <div class=\"progress\">\r\n          <div class=\"progress-bar bg-warning\" style=\"width: 52%\" aria-valuenow=\"52\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </div>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/components/widgets/ratings/ratings.component.scss":
/***/ (function(module, exports) {

module.exports = ".widget-ratings__star {\n  font-size: 1.5rem;\n  color: white;\n  margin: -1.5rem 0 1rem; }\n  .widget-ratings__star .active {\n    color: #ffc107; }\n  .widget-ratings__item {\n  padding: 0.5rem 0; }\n  .widget-ratings__item::after {\n    display: block;\n    clear: both;\n    content: \"\"; }\n  .widget-ratings__item .float-left,\n  .widget-ratings__item .float-right {\n    font-size: 1.15rem; }\n  .widget-ratings__item .float-left .zmdi {\n    font-size: 1.5rem;\n    vertical-align: top;\n    color: #ffc107;\n    position: relative;\n    top: 0.15rem;\n    margin-left: 0.35rem; }\n  .widget-ratings__item:last-child {\n    padding-bottom: 0; }\n  .widget-ratings__progress {\n  overflow: hidden;\n  padding: 0.6rem 1.5rem; }\n"

/***/ }),

/***/ "./src/app/shared/components/widgets/ratings/ratings.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return RatingsComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var RatingsComponent = /** @class */ (function () {
    function RatingsComponent() {
    }
    RatingsComponent.prototype.ngOnInit = function () {
    };
    RatingsComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'widget-ratings',
            template: __webpack_require__("./src/app/shared/components/widgets/ratings/ratings.component.html"),
            styles: [__webpack_require__("./src/app/shared/components/widgets/ratings/ratings.component.scss")]
        }),
        __metadata("design:paramtypes", [])
    ], RatingsComponent);
    return RatingsComponent;
}());



/***/ }),

/***/ "./src/app/shared/components/widgets/recent-posts/recent-posts.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"card\">\r\n    <div class=\"card-body\">\r\n        <h4 class=\"card-title\">Recent Posts</h4>\r\n        <h6 class=\"card-subtitle\">Venenatis portauam Inceptos ameteiam</h6>\r\n    </div>\r\n\r\n    <div class=\"listview listview--hover\">\r\n        <a *ngFor=\"let recentPostData of recentPostData\" class=\"listview__item listview__item--hover\">\r\n            <img [src]=\"recentPostData.avatar\" class=\"listview__img\" alt=\"\">\r\n\r\n            <div class=\"listview__content\">\r\n                <h3 class=\"listview__heading\">{{ recentPostData.user }}</h3>\r\n                <p>{{ recentPostData.post }}</p>\r\n            </div>\r\n        </a>\r\n\r\n        <a href=\"\" class=\"view-more\">View All Posts</a>\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/components/widgets/recent-posts/recent-posts.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return RecentPostsComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var RecentPostsComponent = /** @class */ (function () {
    function RecentPostsComponent() {
        this.recentPostData = [
            {
                user: 'David Villa Jacobs',
                post: 'Sorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam mattis lobortis sapien non posuere',
                avatar: './assets/demo/img/profile-pics/1.jpg'
            },
            {
                user: 'Candice Barnes',
                post: 'Quisque non tortor ultricies, posuere elit id, lacinia purus curabitur.',
                avatar: './assets/demo/img/profile-pics/2.jpg'
            },
            {
                user: 'Jeannette Lawson',
                post: 'Donec congue tempus ligula, varius hendrerit mi hendrerit sit amet. Duis ac quam sit amet leo feugiat iaculis',
                avatar: './assets/demo/img/profile-pics/3.jpg'
            },
            {
                user: 'Darla Mckinney',
                post: 'Duis tincidunt augue nec sem dignissim scelerisque. Vestibulum rhoncus sapien sed nulla aliquam lacinia',
                avatar: './assets/demo/img/profile-pics/4.jpg'
            },
            {
                user: 'Rudolph Perez',
                post: 'Phasellus a ullamcorper lectus, sit amet viverra quam. In luctus tortor vel nulla pharetra bibendum',
                avatar: './assets/demo/img/profile-pics/5.jpg'
            },
        ];
    }
    RecentPostsComponent.prototype.ngOnInit = function () {
    };
    RecentPostsComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'widget-recent-posts',
            template: __webpack_require__("./src/app/shared/components/widgets/recent-posts/recent-posts.component.html")
        }),
        __metadata("design:paramtypes", [])
    ], RecentPostsComponent);
    return RecentPostsComponent;
}());



/***/ }),

/***/ "./src/app/shared/components/widgets/recent-signups/recent-signups.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"card card--inverse widget-signups\">\r\n  <div class=\"card-body\">\r\n    <h4 class=\"card-title\">Most Recent Signups</h4>\r\n    <h6 class=\"card-subtitle\">Magna cursus malesuada lacinia</h6>\r\n\r\n    <div class=\"actions actions--inverse\">\r\n      <div dropdown class=\"actions__item\">\r\n        <i dropdownToggle class=\"zmdi zmdi-more-vert\"></i>\r\n        <div *dropdownMenu class=\"dropdown-menu dropdown-menu-right\">\r\n          <a [routerLink]=\"['/widgets']\" class=\"dropdown-item\">Refresh</a>\r\n          <a [routerLink]=\"['/widgets']\" class=\"dropdown-item\">Manage Widgets</a>\r\n          <a [routerLink]=\"['/widgets']\" class=\"dropdown-item\">Settings</a>\r\n        </div>\r\n      </div>\r\n    </div>\r\n\r\n    <div class=\"widget-signups__chart mt-2\">\r\n      <sparkline [options]=\"signupChartOptions\" [dataset]=\"signupChartData\"></sparkline>\r\n    </div>\r\n  </div>\r\n\r\n  <div class=\"widget-signups__list\">\r\n    <a *ngFor=\"let signupData of signupData\" [routerLink]=\"['/widgets']\" >\r\n      <div *ngIf=\"!signupData.avatar\" class=\"avatar-char\">{{ signupData.letter }}</div>\r\n      <img *ngIf=\"signupData.avatar\" class=\"avatar-img\" [src]=\"signupData.avatar\" alt=\"\">\r\n    </a>\r\n  </div>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/components/widgets/recent-signups/recent-signups.component.scss":
/***/ (function(module, exports) {

module.exports = ".widget-signups {\n  background-color: #607D8B; }\n\n.widget-signups__list {\n  text-align: center;\n  padding: 2rem;\n  background-color: rgba(255, 255, 255, 0.05); }\n\n.widget-signups__list > a {\n    vertical-align: top;\n    margin: 4px 2px;\n    display: inline-block; }\n\n.widget-signups__list .avatar-img,\n  .widget-signups__list .avatar-char {\n    margin: 0; }\n\n.widget-signups__list .avatar-char {\n    background-color: rgba(255, 255, 255, 0.1);\n    color: #FFFFFF; }\n"

/***/ }),

/***/ "./src/app/shared/components/widgets/recent-signups/recent-signups.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return RecentSignupsComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var RecentSignupsComponent = /** @class */ (function () {
    function RecentSignupsComponent() {
        this.signupChartOptions = {
            type: 'line',
            width: '100%',
            height: 50,
            lineColor: 'rgba(255, 255, 255, 0.8)',
            fillColor: 'rgba(0,0,0,0)',
            lineWidth: 1,
            maxSpotColor: '#fff',
            minSpotColor: '#fff',
            spotColor: '#fff',
            spotRadius: 4,
            highlightSpotColor: '#fff',
            highlightLineColor: '#fff'
        };
        this.signupChartData = [9, 4, 6, 5, 6, 4, 5, 7, 9, 3, 6, 5, 9];
        this.signupData = [
            {
                name: 'Clint Hugh',
                letter: 'C',
                avatar: './assets/demo/img/profile-pics/1.jpg'
            },
            {
                name: 'Roydon Jem',
                letter: 'R',
                avatar: ''
            },
            {
                name: 'Wynne John',
                letter: 'W',
                avatar: ''
            },
            {
                name: 'Nicholas Roydon',
                letter: 'N',
                avatar: './assets/demo/img/profile-pics/2.jpg'
            },
            {
                name: 'Wat Shaw',
                letter: 'W',
                avatar: './assets/demo/img/profile-pics/3.jpg'
            },
            {
                name: 'Devereux Brad',
                letter: 'D',
                avatar: ''
            },
            {
                name: 'Fulk Delmar',
                letter: 'F',
                avatar: ''
            },
            {
                name: 'Silver Mathew',
                letter: 'S',
                avatar: ''
            },
            {
                name: 'Geffrey Cortney',
                letter: 'G',
                avatar: ''
            },
            {
                name: 'Lonny Dustin',
                letter: 'L',
                avatar: ''
            },
            {
                name: 'Jaycob Ronny',
                letter: 'J',
                avatar: './assets/demo/img/profile-pics/4.jpg'
            },
            {
                name: 'Alvin Norman',
                letter: 'A',
                avatar: './assets/demo/img/profile-pics/5.jpg'
            },
            {
                name: 'Malcom Dutch',
                letter: 'M',
                avatar: ''
            },
            {
                name: 'Cole Ferdinand',
                letter: 'C',
                avatar: ''
            },
            {
                name: 'Pierce Colin',
                letter: 'P',
                avatar: ''
            },
            {
                name: 'Archibald Frederick',
                letter: 'A',
                avatar: ''
            },
            {
                name: 'Sydney Troy',
                letter: 'S',
                avatar: ''
            },
            {
                name: 'Benji Braxton',
                letter: 'B',
                avatar: './assets/demo/img/profile-pics/6.jpg'
            },
            {
                name: 'Chesley Donny',
                letter: 'C',
                avatar: ''
            },
            {
                name: 'Nate Vere',
                letter: 'N',
                avatar: ''
            },
            {
                name: 'Sammie Roy',
                letter: 'S',
                avatar: ''
            },
            {
                name: 'Sebastian Erik',
                letter: 'S',
                avatar: ''
            }, {
                name: 'Maria Mack',
                letter: 'M',
                avatar: ''
            },
            {
                name: 'Sylvanus Delano',
                letter: 'S',
                avatar: './assets/demo/img/profile-pics/7.jpg'
            }
        ];
    }
    RecentSignupsComponent.prototype.ngOnInit = function () {
    };
    RecentSignupsComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'widget-recent-signups',
            template: __webpack_require__("./src/app/shared/components/widgets/recent-signups/recent-signups.component.html"),
            styles: [__webpack_require__("./src/app/shared/components/widgets/recent-signups/recent-signups.component.scss")]
        }),
        __metadata("design:paramtypes", [])
    ], RecentSignupsComponent);
    return RecentSignupsComponent;
}());



/***/ }),

/***/ "./src/app/shared/components/widgets/tasks/tasks.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"card\">\r\n    <div class=\"card-header\">\r\n        <h2 class=\"card-title\">Ongoing Tasks</h2>\r\n        <small class=\"card-subtitle\">Maecenas seddiam eget risusvarius blandit</small>\r\n    </div>\r\n\r\n    <div class=\"listview\">\r\n        <a href=\"\" class=\"listview__item\">\r\n            <div class=\"listview__content\">\r\n                <div class=\"listview__heading\">HTML5 Validation Report</div>\r\n\r\n                <div class=\"progress\">\r\n                    <div class=\"progress-bar\" style=\"width: 75%\" aria-valuenow=\"75\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>\r\n                </div>\r\n            </div>\r\n        </a>\r\n\r\n        <a href=\"\" class=\"listview__item\">\r\n            <div class=\"listview__content\">\r\n                <div class=\"listview__heading\">Google Chrome Extension</div>\r\n\r\n                <div class=\"progress\">\r\n                    <div class=\"progress-bar bg-warning\" style=\"width: 43%\" aria-valuenow=\"43\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>\r\n                </div>\r\n            </div>\r\n        </a>\r\n\r\n        <a href=\"\" class=\"listview__item\">\r\n            <div class=\"listview__content\">\r\n                <div class=\"listview__heading\">Social Intranet Projects</div>\r\n\r\n                <div class=\"progress\">\r\n                    <div class=\"progress-bar bg-success\" style=\"width: 20%\" aria-valuenow=\"20\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>\r\n                </div>\r\n            </div>\r\n        </a>\r\n\r\n        <a href=\"\" class=\"listview__item\">\r\n            <div class=\"listview__content\">\r\n                <div class=\"listview__heading\">Bootstrap Admin Template</div>\r\n\r\n                <div class=\"progress\">\r\n                    <div class=\"progress-bar bg-info\" style=\"width: 60%\" aria-valuenow=\"60\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>\r\n                </div>\r\n            </div>\r\n        </a>\r\n\r\n        <a href=\"\" class=\"listview__item\">\r\n            <div class=\"listview__content\">\r\n                <div class=\"listview__heading\">Youtube Client App</div>\r\n\r\n                <div class=\"progress\">\r\n                    <div class=\"progress-bar bg-danger\" style=\"width: 80%\" aria-valuenow=\"80\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>\r\n                </div>\r\n            </div>\r\n        </a>\r\n\r\n        <a href=\"\" class=\"view-more\">View All Tasks</a>\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/components/widgets/tasks/tasks.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return TasksComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var TasksComponent = /** @class */ (function () {
    function TasksComponent() {
    }
    TasksComponent.prototype.ngOnInit = function () {
    };
    TasksComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'widget-tasks',
            template: __webpack_require__("./src/app/shared/components/widgets/tasks/tasks.component.html")
        }),
        __metadata("design:paramtypes", [])
    ], TasksComponent);
    return TasksComponent;
}());



/***/ }),

/***/ "./src/app/shared/components/widgets/todo-lists/todo-lists.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"card todo\">\r\n    <div class=\"card-body\">\r\n        <h4 class=\"card-title\">Todo lists</h4>\r\n        <h6 class=\"card-subtitle\">Venenatis portauam Inceptos ameteiam</h6>\r\n    </div>\r\n\r\n    <div class=\"listview\">\r\n        <div *ngFor=\"let todoListData of todoListData\" class=\"listview__item\">\r\n            <label class=\"custom-control custom-control--char todo__item\">\r\n                <input class=\"custom-control-input\" type=\"checkbox\" value=\"\" checked=\"{{ todoListData.checked }}\">\r\n                <span class=\"custom-control--char__helper\"><i class=\"{{ todoListData.color }}\">{{ todoListData.letter }}</i></span>\r\n                <div class=\"todo__info\">\r\n                    <span>{{ todoListData.todo }}</span>\r\n                    <small>{{ todoListData.venue }}</small>\r\n                </div>\r\n\r\n                <div class=\"listview__attrs\">\r\n                    <span>{{ todoListData.category }}</span>\r\n                    <span>{{ todoListData.priority }}</span>\r\n                </div>\r\n            </label>\r\n\r\n            <div class=\"actions listview__actions\">\r\n                <div dropdown class=\"actions__item\">\r\n                    <i dropdownToggle class=\"zmdi zmdi-more-vert\"></i>\r\n                    <div *dropdownMenu class=\"dropdown-menu dropdown-menu-right\">\r\n                        <a class=\"dropdown-item\" [routerLink]=\"['/home']\">Mark as completed</a>\r\n                        <a class=\"dropdown-item\" [routerLink]=\"['/home']\">Delete</a>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n\r\n        <a href=\"\" class=\"view-more\">View More</a>\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/components/widgets/todo-lists/todo-lists.component.scss":
/***/ (function(module, exports) {

module.exports = ".todo__item {\n  padding-left: 4.5rem;\n  display: block; }\n  .todo__item small {\n    display: block;\n    font-size: 0.95rem;\n    margin-top: 0.2rem; }\n  .todo__item .custom-control-input:checked ~ .todo__info {\n    text-decoration: line-through; }\n  .todo__item .custom-control-input:checked ~ .custom-control--char__helper > i {\n    background-color: #e9e9e9 !important; }\n  .todo__item .custom-control-input:checked ~ .custom-control--char__helper:after {\n    color: #5E5E5E; }\n  .todo__info > span {\n  color: #333; }\n  .todo__info > small {\n  color: #9c9c9c; }\n  .todo__labels > a {\n  color: #ffc107;\n  border: 2px solid #ffdb6d;\n  border-radius: 2px;\n  padding: 0.35rem 0.8rem;\n  display: inline-block;\n  margin: 0 0.1rem 0.4rem;\n  -webkit-transition: color 300ms, border-color 300ms;\n  transition: color 300ms, border-color 300ms; }\n  .todo__labels > a:hover {\n    color: #edb100; }\n"

/***/ }),

/***/ "./src/app/shared/components/widgets/todo-lists/todo-lists.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return TodoListsComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var TodoListsComponent = /** @class */ (function () {
    function TodoListsComponent() {
        this.todoListData = [
            {
                letter: 'F',
                todo: 'Fivamus sagittis lacus vel augue laoreet rutrum faucibus dolor',
                venue: 'Today at 8.30 AM',
                category: '#Messages',
                priority: '!!!',
                checked: 'checked',
                color: 'bg-amber'
            },
            {
                letter: 'N',
                todo: 'Nullam id dolor id nibh ultricies vehicula ut id elit',
                venue: 'Today at 12.30 PM',
                category: '#Clients',
                priority: '!!',
                checked: 'checked',
                color: 'bg-light-blue'
            },
            {
                letter: 'C',
                todo: 'Cras mattis consectetur purus sit amet fermentum',
                venue: 'Tomorrow at 10.30 AM',
                category: '#Clients',
                priority: '!!',
                completed: '',
                color: 'bg-purple'
            },
            {
                letter: 'I',
                todo: 'Integer posuere erat a ante venenatis dapibus posuere velit aliquet',
                venue: '05/08/2017 at 08.00 AM',
                category: '#Server',
                priority: '!',
                completed: '',
                color: 'bg-lime'
            },
            {
                letter: 'P',
                todo: 'Praesent commodo cursus magnavel scelerisque nisl consectetur',
                venue: '10/08/2016 at 04.00 AM',
                category: '#Server',
                priority: '!!!',
                completed: '',
                color: 'bg-red'
            },
        ];
    }
    TodoListsComponent.prototype.ngOnInit = function () {
    };
    TodoListsComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'widget-todo-lists',
            template: __webpack_require__("./src/app/shared/components/widgets/todo-lists/todo-lists.component.html"),
            styles: [__webpack_require__("./src/app/shared/components/widgets/todo-lists/todo-lists.component.scss")]
        }),
        __metadata("design:paramtypes", [])
    ], TodoListsComponent);
    return TodoListsComponent;
}());



/***/ }),

/***/ "./src/app/shared/components/widgets/visitors/visitors.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"card widget-visitors\">\r\n    <div class=\"card-body\">\r\n        <h4 class=\"card-title\">Realtime Visitors</h4>\r\n        <h6 class=\"card-subtitle\">Nullam dolor isnibh ultricies vehicula adipiscing</h6>\r\n\r\n        <div class=\"widget-visitors__stats\">\r\n            <div>\r\n                <strong>23528</strong>\r\n                <small>Visitor for last 24 hours</small>\r\n            </div>\r\n            <div>\r\n                <strong>746</strong>\r\n                <small>Visitors last 30 minutes</small>\r\n            </div>\r\n        </div>\r\n\r\n        <div JqvMap [options]=\"visitorsMap\" [height]=\"250\"></div>\r\n    </div>\r\n\r\n    <div class=\"listview listview--bordered\">\r\n        <div *ngFor=\"let visitorData of visitorsData\" class=\"listview__item\">\r\n            <div class=\"listview__content\">\r\n                <p>{{ visitorData.date }}</p>\r\n\r\n                <div class=\"listview__attrs\">\r\n                    <span><img class=\"widget-visitors__country\" [src]=\"visitorData.img\" alt=\"\"> {{ visitorData.country }}</span>\r\n                    <span>{{ visitorData.browser }}</span>\r\n                    <span>{{ visitorData.os }}</span>\r\n                </div>\r\n            </div>\r\n        </div>\r\n\r\n        <a href=\"\" class=\"view-more\">View All</a>\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/components/widgets/visitors/visitors.component.scss":
/***/ (function(module, exports) {

module.exports = ".widget-visitors__stats {\n  margin: 0 -0.5rem 2rem; }\n  .widget-visitors__stats::after {\n    display: block;\n    clear: both;\n    content: \"\"; }\n  .widget-visitors__stats > div {\n    border: 1px solid white;\n    padding: 1.1rem 1.5rem;\n    float: left;\n    margin: 0 0.5rem;\n    width: calc(50% - 1rem); }\n  .widget-visitors__stats > div > strong {\n      font-size: 1.9rem;\n      font-weight: normal;\n      line-height: 100%;\n      color: #333; }\n  .widget-visitors__stats > div > small {\n      display: block;\n      color: #9c9c9c;\n      font-size: 0.9rem;\n      line-height: 100%;\n      margin-top: 0.25rem; }\n  .widget-visitors__country {\n  height: 1rem;\n  width: 1.5rem;\n  vertical-align: top;\n  position: relative;\n  margin-right: 0.25rem;\n  left: -0.1rem;\n  border-radius: 1px; }\n"

/***/ }),

/***/ "./src/app/shared/components/widgets/visitors/visitors.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return VisitorsComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var VisitorsComponent = /** @class */ (function () {
    function VisitorsComponent() {
        this.visitorsMap = {
            map: 'world_en',
            backgroundColor: '#fff',
            color: '#ebebeb',
            borderColor: '#ebebeb',
            hoverOpacity: 1,
            selectedColor: '#00BCD4',
            enableZoom: false,
            showTooltip: true,
            normalizeFunction: 'polynomial',
            selectedRegions: ['US', 'EN', 'NZ', 'CN', 'JP', 'SL', 'BR', 'AU'],
            onRegionClick: function (event) {
                event.preventDefault();
            }
        };
        this.visitorsData = [
            {
                date: 'Sunday, September 4, 21:44:02 (2 Mins 56 Seconds)',
                country: 'United States',
                browser: 'Firefox',
                os: 'Mac OSX',
                img: './assets/demo/img/flags/United_States_of_America.png'
            },
            {
                date: 'Sunday, September 4, 20:21:01 (5 Mins 12 Seconds)',
                country: 'Australia',
                browser: 'Chrome',
                os: 'Android',
                img: './assets/demo/img/flags/Australia.png'
            },
            {
                date: 'Sunday, September 4, 20:21:10 (10 Mins 43 Seconds)',
                country: 'Brazil',
                browser: 'Edge',
                os: 'Windows',
                img: './assets/demo/img/flags/Brazil.png'
            },
            {
                date: 'Sunday, September 4, 20:59:04 (1 Min 02 Seconds)',
                country: 'South Korea',
                browser: 'Chrome',
                os: 'Android',
                img: './assets/demo/img/flags/South_Korea.png'
            },
            {
                date: 'Sunday, September 4, 20:58:12 (3 Min 44 Seconds)',
                country: 'Japan',
                browser: 'Chrome',
                os: 'Windows',
                img: './assets/demo/img/flags/Japan.png'
            }
        ];
    }
    VisitorsComponent.prototype.ngOnInit = function () {
    };
    VisitorsComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'widget-visitors',
            template: __webpack_require__("./src/app/shared/components/widgets/visitors/visitors.component.html"),
            styles: [__webpack_require__("./src/app/shared/components/widgets/visitors/visitors.component.scss")]
        }),
        __metadata("design:paramtypes", [])
    ], VisitorsComponent);
    return VisitorsComponent;
}());



/***/ }),

/***/ "./src/app/shared/directives/dropzone/dropzone.directive.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return DropzoneDirective; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
/// <reference types="dropzone/"/>

var DropzoneDirective = /** @class */ (function () {
    function DropzoneDirective(el) {
        this.el = el;
    }
    DropzoneDirective.prototype.ngOnInit = function () {
        var initDropzone = jQuery(this.el.nativeElement);
        initDropzone.dropzone({
            url: this.posturl,
            addRemoveLinks: true
        });
    };
    __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(),
        __metadata("design:type", Object)
    ], DropzoneDirective.prototype, "posturl", void 0);
    DropzoneDirective = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Directive"])({
            selector: '[dropzone]'
        }),
        __metadata("design:paramtypes", [__WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"]])
    ], DropzoneDirective);
    return DropzoneDirective;
}());



/***/ }),

/***/ "./src/app/shared/directives/easy-pie-chart/easy-pie-chart.directive.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return EasyPieChartDirective; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var EasyPieChartDirective = /** @class */ (function () {
    function EasyPieChartDirective(el) {
        this.el = el;
    }
    EasyPieChartDirective.prototype.ngOnInit = function () {
        var initEasyPieChart = jQuery(this.el.nativeElement);
        initEasyPieChart.find('.easy-pie-chart__value').css({
            lineHeight: (this.size) + 'px',
            fontSize: (this.size / 4) + 'px'
        });
        initEasyPieChart.easyPieChart({
            easing: 'easeOutBounce',
            barColor: '#fff',
            trackColor: 'rgba(0,0,0,0.08)',
            scaleColor: 'rgba(0,0,0,0)',
            lineCap: 'round',
            lineWidth: 2,
            size: this.size,
            animate: 3000
        });
    };
    __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(),
        __metadata("design:type", Object)
    ], EasyPieChartDirective.prototype, "size", void 0);
    EasyPieChartDirective = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Directive"])({
            selector: '[EasyPieChart]'
        }),
        __metadata("design:paramtypes", [__WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"]])
    ], EasyPieChartDirective);
    return EasyPieChartDirective;
}());



/***/ }),

/***/ "./src/app/shared/directives/flot/flot.directive.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return FlotDirective; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var FlotDirective = /** @class */ (function () {
    function FlotDirective(el) {
        this.el = el;
        this.initFlot = jQuery(this.el.nativeElement);
    }
    FlotDirective.prototype.onResize = function () {
        this.initFlot.plot(this.dataset, this.options);
    };
    FlotDirective.prototype.ngOnInit = function () {
        this.initFlot.css({
            height: this.height
        });
        jQuery.plot(this.initFlot, this.dataset, this.options);
        // Tooltips
        this.initFlot.on('plothover', function (event, pos, item) {
            if (item) {
                var x = item.datapoint[0].toFixed(2), y = item.datapoint[1].toFixed(2);
                jQuery('.flot-tooltip').html(item.series.label + ' of ' + x + ' = ' + y).css({ top: item.pageY + 5, left: item.pageX + 5 }).show();
            }
            else {
                jQuery('.flot-tooltip').hide();
            }
        });
        jQuery('body').append('<div class="flot-tooltip" />');
    };
    __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(),
        __metadata("design:type", Object)
    ], FlotDirective.prototype, "options", void 0);
    __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(),
        __metadata("design:type", Object)
    ], FlotDirective.prototype, "dataset", void 0);
    __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(),
        __metadata("design:type", String)
    ], FlotDirective.prototype, "height", void 0);
    FlotDirective = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Directive"])({
            selector: '[flot]',
            host: {
                '(window:resize)': 'onResize($event)'
            }
        }),
        __metadata("design:paramtypes", [__WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"]])
    ], FlotDirective);
    return FlotDirective;
}());



/***/ }),

/***/ "./src/app/shared/directives/input-float/input-float.directive.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return InputFloatDirective; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var InputFloatDirective = /** @class */ (function () {
    function InputFloatDirective(el, renderer) {
        this.el = el;
        this.renderer = renderer;
        this.elem = this.el.nativeElement;
    }
    InputFloatDirective.prototype.onBlur = function () {
        console.log("INPUT VAL: ", this.elem.value);
        var status = true ? this.elem.value : false;
        this.renderer.setElementClass(this.elem, 'form-control--active', status);
    };
    InputFloatDirective.prototype.ngOnInit = function () {
        console.log("INPUT VAL: ", this.elem.value);
        if (this.elem.value) {
            this.renderer.setElementClass(this.elem, 'form-control--active', true);
        }
    };
    InputFloatDirective = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Directive"])({
            selector: '[formControlFloat]',
            host: {
                '(blur)': 'onBlur()'
            }
        }),
        __metadata("design:paramtypes", [__WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"], __WEBPACK_IMPORTED_MODULE_0__angular_core__["Renderer"]])
    ], InputFloatDirective);
    return InputFloatDirective;
}());



/***/ }),

/***/ "./src/app/shared/directives/jqvmap/jqvmap.directive.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return JqvMapDirective; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var JqvMapDirective = /** @class */ (function () {
    function JqvMapDirective(el) {
        this.el = el;
    }
    JqvMapDirective.prototype.ngOnInit = function () {
        var initJqvMap = jQuery(this.el.nativeElement);
        initJqvMap.css({
            height: this.height,
            width: '100%'
        });
        initJqvMap.vectorMap(this.options);
    };
    __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(),
        __metadata("design:type", Object)
    ], JqvMapDirective.prototype, "options", void 0);
    __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(),
        __metadata("design:type", Object)
    ], JqvMapDirective.prototype, "height", void 0);
    JqvMapDirective = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Directive"])({
            selector: '[JqvMap]'
        }),
        __metadata("design:paramtypes", [__WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"]])
    ], JqvMapDirective);
    return JqvMapDirective;
}());



/***/ }),

/***/ "./src/app/shared/directives/sparklines/sparkline.directive.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return SparklineDirective; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var SparklineDirective = /** @class */ (function () {
    function SparklineDirective(el) {
        this.el = el;
    }
    SparklineDirective.prototype.ngOnInit = function () {
        var initSparkline = jQuery(this.el.nativeElement);
        // Initiate Sparkline Chart
        initSparkline.sparkline(this.dataset, this.options);
    };
    __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(),
        __metadata("design:type", String)
    ], SparklineDirective.prototype, "type", void 0);
    __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(),
        __metadata("design:type", Object)
    ], SparklineDirective.prototype, "options", void 0);
    __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(),
        __metadata("design:type", Object)
    ], SparklineDirective.prototype, "dataset", void 0);
    SparklineDirective = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Directive"])({
            selector: 'sparkline'
        }),
        __metadata("design:paramtypes", [__WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"]])
    ], SparklineDirective);
    return SparklineDirective;
}());



/***/ }),

/***/ "./src/app/shared/shared.module.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return SharedModule; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_common__ = __webpack_require__("./node_modules/@angular/common/esm5/common.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__("./node_modules/@angular/core/esm5/core.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_router__ = __webpack_require__("./node_modules/@angular/router/esm5/router.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_ngx_bootstrap_dropdown__ = __webpack_require__("./node_modules/ngx-bootstrap/dropdown/index.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__directives_easy_pie_chart_easy_pie_chart_directive__ = __webpack_require__("./src/app/shared/directives/easy-pie-chart/easy-pie-chart.directive.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__directives_jqvmap_jqvmap_directive__ = __webpack_require__("./src/app/shared/directives/jqvmap/jqvmap.directive.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__directives_flot_flot_directive__ = __webpack_require__("./src/app/shared/directives/flot/flot.directive.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__directives_sparklines_sparkline_directive__ = __webpack_require__("./src/app/shared/directives/sparklines/sparkline.directive.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__directives_dropzone_dropzone_directive__ = __webpack_require__("./src/app/shared/directives/dropzone/dropzone.directive.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__directives_input_float_input_float_directive__ = __webpack_require__("./src/app/shared/directives/input-float/input-float.directive.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__components_widgets_todo_lists_todo_lists_component__ = __webpack_require__("./src/app/shared/components/widgets/todo-lists/todo-lists.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_11__components_widgets_pie_charts_pie_charts_component__ = __webpack_require__("./src/app/shared/components/widgets/pie-charts/pie-charts.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_12__components_widgets_random_post_random_post_component__ = __webpack_require__("./src/app/shared/components/widgets/random-post/random-post.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_13__components_widgets_recent_posts_recent_posts_component__ = __webpack_require__("./src/app/shared/components/widgets/recent-posts/recent-posts.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_14__components_widgets_visitors_visitors_component__ = __webpack_require__("./src/app/shared/components/widgets/visitors/visitors.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_15__components_widgets_past_days_past_days_component__ = __webpack_require__("./src/app/shared/components/widgets/past-days/past-days.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_16__components_widgets_photos_photos_component__ = __webpack_require__("./src/app/shared/components/widgets/photos/photos.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_17__components_widgets_tasks_tasks_component__ = __webpack_require__("./src/app/shared/components/widgets/tasks/tasks.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_18__components_widgets_contacts_contacts_component__ = __webpack_require__("./src/app/shared/components/widgets/contacts/contacts.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_19__components_widgets_ratings_ratings_component__ = __webpack_require__("./src/app/shared/components/widgets/ratings/ratings.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_20__components_widgets_profile_profile_component__ = __webpack_require__("./src/app/shared/components/widgets/profile/profile.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_21__components_widgets_recent_signups_recent_signups_component__ = __webpack_require__("./src/app/shared/components/widgets/recent-signups/recent-signups.component.ts");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};






















var SharedModule = /** @class */ (function () {
    function SharedModule() {
    }
    SharedModule = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_1__angular_core__["NgModule"])({
            declarations: [
                // Directives
                __WEBPACK_IMPORTED_MODULE_4__directives_easy_pie_chart_easy_pie_chart_directive__["a" /* EasyPieChartDirective */],
                __WEBPACK_IMPORTED_MODULE_5__directives_jqvmap_jqvmap_directive__["a" /* JqvMapDirective */],
                __WEBPACK_IMPORTED_MODULE_6__directives_flot_flot_directive__["a" /* FlotDirective */],
                __WEBPACK_IMPORTED_MODULE_7__directives_sparklines_sparkline_directive__["a" /* SparklineDirective */],
                __WEBPACK_IMPORTED_MODULE_8__directives_dropzone_dropzone_directive__["a" /* DropzoneDirective */],
                __WEBPACK_IMPORTED_MODULE_9__directives_input_float_input_float_directive__["a" /* InputFloatDirective */],
                // Components
                __WEBPACK_IMPORTED_MODULE_15__components_widgets_past_days_past_days_component__["a" /* PastDaysComponent */],
                __WEBPACK_IMPORTED_MODULE_10__components_widgets_todo_lists_todo_lists_component__["a" /* TodoListsComponent */],
                __WEBPACK_IMPORTED_MODULE_11__components_widgets_pie_charts_pie_charts_component__["a" /* PieChartsComponent */],
                __WEBPACK_IMPORTED_MODULE_12__components_widgets_random_post_random_post_component__["a" /* RandomPostComponent */],
                __WEBPACK_IMPORTED_MODULE_13__components_widgets_recent_posts_recent_posts_component__["a" /* RecentPostsComponent */],
                __WEBPACK_IMPORTED_MODULE_14__components_widgets_visitors_visitors_component__["a" /* VisitorsComponent */],
                __WEBPACK_IMPORTED_MODULE_16__components_widgets_photos_photos_component__["a" /* PhotosComponent */],
                __WEBPACK_IMPORTED_MODULE_17__components_widgets_tasks_tasks_component__["a" /* TasksComponent */],
                __WEBPACK_IMPORTED_MODULE_18__components_widgets_contacts_contacts_component__["a" /* ContactsComponent */],
                __WEBPACK_IMPORTED_MODULE_19__components_widgets_ratings_ratings_component__["a" /* RatingsComponent */],
                __WEBPACK_IMPORTED_MODULE_20__components_widgets_profile_profile_component__["a" /* ProfileComponent */],
                __WEBPACK_IMPORTED_MODULE_21__components_widgets_recent_signups_recent_signups_component__["a" /* RecentSignupsComponent */]
            ],
            imports: [
                __WEBPACK_IMPORTED_MODULE_0__angular_common__["CommonModule"],
                __WEBPACK_IMPORTED_MODULE_2__angular_router__["c" /* RouterModule */],
                __WEBPACK_IMPORTED_MODULE_3_ngx_bootstrap_dropdown__["a" /* BsDropdownModule */].forRoot()
            ],
            exports: [
                // Directives
                __WEBPACK_IMPORTED_MODULE_4__directives_easy_pie_chart_easy_pie_chart_directive__["a" /* EasyPieChartDirective */],
                __WEBPACK_IMPORTED_MODULE_5__directives_jqvmap_jqvmap_directive__["a" /* JqvMapDirective */],
                __WEBPACK_IMPORTED_MODULE_6__directives_flot_flot_directive__["a" /* FlotDirective */],
                __WEBPACK_IMPORTED_MODULE_7__directives_sparklines_sparkline_directive__["a" /* SparklineDirective */],
                __WEBPACK_IMPORTED_MODULE_8__directives_dropzone_dropzone_directive__["a" /* DropzoneDirective */],
                __WEBPACK_IMPORTED_MODULE_9__directives_input_float_input_float_directive__["a" /* InputFloatDirective */],
                // Components
                __WEBPACK_IMPORTED_MODULE_15__components_widgets_past_days_past_days_component__["a" /* PastDaysComponent */],
                __WEBPACK_IMPORTED_MODULE_10__components_widgets_todo_lists_todo_lists_component__["a" /* TodoListsComponent */],
                __WEBPACK_IMPORTED_MODULE_11__components_widgets_pie_charts_pie_charts_component__["a" /* PieChartsComponent */],
                __WEBPACK_IMPORTED_MODULE_12__components_widgets_random_post_random_post_component__["a" /* RandomPostComponent */],
                __WEBPACK_IMPORTED_MODULE_13__components_widgets_recent_posts_recent_posts_component__["a" /* RecentPostsComponent */],
                __WEBPACK_IMPORTED_MODULE_14__components_widgets_visitors_visitors_component__["a" /* VisitorsComponent */],
                __WEBPACK_IMPORTED_MODULE_16__components_widgets_photos_photos_component__["a" /* PhotosComponent */],
                __WEBPACK_IMPORTED_MODULE_17__components_widgets_tasks_tasks_component__["a" /* TasksComponent */],
                __WEBPACK_IMPORTED_MODULE_18__components_widgets_contacts_contacts_component__["a" /* ContactsComponent */],
                __WEBPACK_IMPORTED_MODULE_19__components_widgets_ratings_ratings_component__["a" /* RatingsComponent */],
                __WEBPACK_IMPORTED_MODULE_20__components_widgets_profile_profile_component__["a" /* ProfileComponent */],
                __WEBPACK_IMPORTED_MODULE_21__components_widgets_recent_signups_recent_signups_component__["a" /* RecentSignupsComponent */],
            ]
        })
    ], SharedModule);
    return SharedModule;
}());



/***/ })

});
//# sourceMappingURL=home.module.chunk.js.map