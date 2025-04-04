class Rectangle {
    constructor(height, width) {
      this.height = height;
      this.width = width;
    }
    // Getter
    get area() {
      return this.calcArea();
    }

    get perimetro() {
      return this.calcPerimetro();
    }
    // Method
    calcArea() {
      return this.height * this.width;
    }

    calcPerimetro() {
      return 2 * (this.height + this.width);
    }

    *getSides() {
      yield this.height;
      yield this.width;
      yield this.height;
      yield this.width;
    }
  }
  
  const square = new Rectangle(10, 10);
  
  console.log(square.area); // 100
  console.log([...square.getSides()]); // [10, 10, 10, 10]
  console.log(square.perimetro); // 40