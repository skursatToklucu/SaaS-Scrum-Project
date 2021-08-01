
var KTBootstrapDatepicker = function () {

 var arrows;
 if (KTUtil.isRTL()) {
  arrows = {
   leftArrow: '<i class="la la-angle-right"></i>',
   rightArrow: '<i class="la la-angle-left"></i>'
  }
 } else {
  arrows = {
   leftArrow: '<i class="la la-angle-left"></i>',
   rightArrow: '<i class="la la-angle-right"></i>'
  }
 }



  // range picker
  $('#kt_datepicker_5').datepicker({
      rtl: KTUtil.isRTL(),
      dateFormat:'dd-mm-yyyy',
       todayHighlight: true,
      templates: arrows,
     
  });

   // inline picker
  $('#kt_datepicker_6').datepicker({
   rtl: KTUtil.isRTL(),
   todayHighlight: true,
   templates: arrows
  });
 }

 return {
  // public functions
  init: function() {
   demos();
  }
 };
}();

jQuery(document).ready(function() {
 KTBootstrapDatepicker.init();
});