using System;
using System.Collections.Generic;
using System.Text;

namespace TestCSharp
{
    // =============================================================
    // 클래스와 구조체의 이름은 파스칼식(PascalCase) 대/소문자를 사용한다.
    // =============================================================
    class SampleClass
    {
        // =============================================================
        // 함수 설명
        // 함수의 이름은 파스칼식(PascalCase) 대/소문자를 사용한다.
        // 매개 변수 이름에는 카멜식(camelCase) 대/소문자를 사용합니다. 
        // 설명이 포함된 매개 변수 이름을 사용합니다.
        // 항상 접근 제한자를 먼저 표기한다. EX: O: private string _foo
        // =============================================================
        public int FuncSample(int camelCase)
        {
            return 0;
        }

        // =============================================================
        // 필드(객체 변수) - Private, Internal
        // _로 시작해서 _camelCase 식을 사용한다.
        // 필드 이름은 명사 또는 명사구를 사용하여 지정합니다.
        // 정적 필드는 s_를 붙인다.
        // thread static 필드는 t_를 붙인다
        // =============================================================
        private Int32 _intValue;
        internal Int32 _sameTypeCastValue;
        private static Int32 s_staticValue;
        private static Int32 t_threadValue;

        // =============================================================
        // 필드(객체 변수) - Public
        // 최대한 쓰지 않는다. - 파스칼식(PascalCase) 대/소문자 사용
        // =============================================================
        public Int32 PublicValue;

        // =============================================================
        // 나머지...
        // 상수는 파스칼식(PascalCase) 대/소문자를 사용한다.
        const Int32 ConstValue = 1;
        // 매개 변수 타입을 기반으로 하는 이름이 아닌, 
        // 매개 변수의 의미를 기반으로 하는 이름을 사용할 수 있습니다.
        // private int intValue (x)
        // private int tempValue (o)
        // 변수, 함수, 인스턴스 등을 하드 프린트하는 대신 nameof("...")을 사용한다
        private void SampleFunc()
        {
            object nameSample;
            string getSample = nameof(nameSample);
        }

        // =============================================================

        // =============================================================
        // 코드블락
        // 올맨 스타일의 각 중괄호는 새로운 줄에서 시작한다. 조건문이 한줄이라도 중괄호는 사용한다
        // 인덴트는 4빈칸을 사용한다. -> 클래스 안의 괄호 같은 경우. 기본이 스페이스 4칸이지만, 가끔 스페이스 2칸을 사용하는 곳도 있음.
        // 빈줄은 최대 1개만 사용한다. -> 가독성용으로 빈준을 사용할 경우 최대 1줄을 사용하라는 말임.
        // 코드 끝자락에 빈칸은 생략한다. -> 엔터를 끝에 사용하지 말라는 말임
        private void AllmanFunc()
        {
            if(true)
            {
                // 한줄 if문이라도 중괄호 사용
            }

            while (true)
            {
                SampleFunc();
                FuncSample(0);
            }
        }
        // =============================================================


        // =============================================================
        //정수 타입은 비트의 크기가 붙은 것을 사용한다. int -> Int32
        private Int32 _fourByteValue;
        // 쉽게 읽을 수 있는 식별자 이름을 선택한다
        // 예를 들어, 영어에서는 AlignmentHorizontal 라는 속성 이름보다 
        // HorizontalAlignment 라는 이름이 읽기가 더 쉽다.

        // 간결성보다는 가독성에 중점을 둔다
        // 예를 들어, 전자의 경우 X 축에 대한 참조가 명확하지 않은 ScrollableX라는 속성 이름보다는 
        // CanScrollHorizontally라는 이름이 더 좋다.
        // 밑줄(_), 하이픈(-) 또는 기타 영숫자가 아닌 문자를 사용하지 않는다.
        // 헝가리어 표기법을 사용하지 않는다. (int iValue)
        // 널리 사용되는 프로그래밍 언어의 키워드와 충돌하는 식별자를 사용하지 않는다.
    }
}