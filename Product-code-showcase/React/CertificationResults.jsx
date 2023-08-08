import React, { useState, useEffect } from "react";
import { Card, Row, Table, Button } from "react-bootstrap";
import { CardBody, Col } from "reactstrap";
import certificationResultsService from "services/certificationResultsService";

import toastr from "toastr";
import TitleHeader from "components/general/TitleHeader";

import MappingCertResults from "./MapCertResults";
import { useParams } from "react-router-dom";
import Pagination from "rc-pagination";
import "rc-pagination/assets/index.css";
import locale from "rc-pagination/lib/locale/en_US";



function CertificationResults() {
  const [data, setData] = useState({
    certArray: [],
    certArrayComponents: [],
    currentCert: "",
    num: 0,
    pageIndex: 0,
    pageSize: 6,
    current: 1,
    totalCount: 1,
  });

  const [search, setSearch] = useState("");

  const { id } = useParams();

  useEffect(() => {
    certificationResultsService
      .getAllById(id, data.pageIndex, data.pageSize)
      .then(onGetAllCertResultSuccess)
      .catch(onGetAllCertResultError);
  }, [data.pageIndex, data.num]);

  const onGetAllCertResultError = (error) => {
    
    toastr.error("Failed to find Certification");
  };
  const onGetAllCertResultSuccess = (data) => {
    const certResultsArray = data.item.pagedItems;

    setData((prevState) => {
      let newData = { ...prevState };
      newData.certArray = certResultsArray;
      newData.certArrayComponents = certResultsArray.map(mapCertification);
      newData.total = data.item.totalCount;
      newData.currentCert = certResultsArray[0]?.certification;
      _logger(newData);
      return newData;
    });
  };

  const mapCertification = (aCertResultsObject) => {
    return (
      <MappingCertResults
        certification={aCertResultsObject}
        onEdit={handleEdit}
        onUpdate={updateCertificationStatus}
        key={"cert-" + aCertResultsObject.id}
      />
    );
  };

  const onPaginationChange = (page) => {
    setData((prevState) => {
      const newCertState = { ...prevState };
      newCertState.current = page;
      newCertState.pageIndex = page - 1;

      return newCertState;
    });
  };

  const searchPage = () => {
    certificationResultsService
      .searchCertResult(data.pageIndex, data.pageSize, search)
      .then(onSearchCertResultSuccess)
      .catch(onSearchCertResultError);
  };
  const onSearchCertResultSuccess = (data) => {
    const certResultsArray = data.item.pagedItems;
    toastr.success("Records Found");

    setData((prevState) => {
      let newData = { ...prevState };
      newData.certArray = certResultsArray;
      newData.certArrayComponents = certResultsArray.map(mapCertification);
      newData.total = data.item.totalCount;
      _logger(newData);
      return newData;
    });
  };

  const updateCertificationStatus = (updateCertResult) => {
    _logger(updateCertResult, "this is before we map it into a payload");
    const payload = {
      id: updateCertResult.id,
      certificationId: updateCertResult.certification.id,
      isPhysicalCompleted:
        updateCertResult.isPhysicalCompleted.toString() === "true"
          ? true
          : false,
      isBackgroundCheckCompleted:
        updateCertResult.isBackgroundCheckCompleted.toString() === "true"
          ? true
          : false,
      isTestCompleted:
        updateCertResult.isTestCompleted.toString() === "true" ? true : false,
      testInstanceId:
        updateCertResult.testInstanceId === 0
          ? null
          : updateCertResult.testInstanceId,
      score: updateCertResult.score === 0 ? null : updateCertResult.score,
      isFitnessTestCompleted:
        updateCertResult.isFitnessTestCompleted.toString() === "true"
          ? true
          : false,
      isClinicAttended:
        updateCertResult.isClinicAttended.toString() === "true" ? true : false,
      isActive: updateCertResult.isActive,
      userId: updateCertResult.user.id,
    };
  
    certificationResultsService
      .updateCertResult(payload)
      .then(onUpdateCertSuccess)
      .catch(onUpdateCertError);
  };

  const onUpdateCertSuccess = (payload) => {
   
    setData((prevState) => {
      let newData = { ...prevState };
      newData.num = newData.num + 1;
      let indexToUpdate = newData.certArray.findIndex(
        (load) => load.id === payload.id
      );
     
      if (indexToUpdate >= 0) {
        
        newData.certArrayComponents = newData.certArray.map(mapCertification);
      }

      return newData;
    });
  };
  const onUpdateCertError = (error) => {
    toastr.error("Failed to update cert", error);
  };

  const onSearchCertResultError = (error) => {
    const errorMessage = "  No Records Found";
    

    setData((prevState) => ({
      ...prevState,
      certArray: [errorMessage],
      certArrayComponents: [errorMessage],
      total: 0,
    }));
  };

  const handleSearchButtonClick = () => {
    if (search.trim() === "") {
      toastr.error("Invalid search value");
      return;
    }
    if (!/^[a-zA-Z]+$/.test(search.trim())) {
      toastr.error("Invalid search value");
    }
    searchPage();
  };
  const handleEdit = (certification) => {
   
  };

  return (
    <React.Fragment>
      <TitleHeader title="Certification Results" />
      <Row>
        <Col>
          <Card>
            <CardBody className="p-0">
              <Row className="p-2 ">
                <Col className="col-9  pt-2 ">
                  <h3 className="px-3">{data.currentCert.name}</h3>
                </Col>
                <Col className=" h-25 col-3 d-flex text-right  ">
                  <input
                    className="form-control form-control-sm text-primary"
                    type="text"
                    placeholder="Search by Name"
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                  />
                  <Button
                    variant="btn btn-outline-secondary btn-sm"
                    onClick={handleSearchButtonClick}
                  >
                    Search
                  </Button>
                </Col>
              </Row>

              <div className="table-responsive-lg">
                <Table className="table table-stripped table-hover">
                  <thead className="table-light">
                    <tr
                      role="row"
                      className=" align-middle "
                      style={{ backgroundColor: "whitesmoke" }}
                    >
                      <th colSpan="1" role="columnheader">
                        UserName
                      </th>
                      {data.currentCert.isPhysicalRequired ? (
                        <th
                          className="text-center "
                          colSpan="1"
                          role="columnheader"
                        >
                          Physical Completed
                        </th>
                      ) : (
                        <th colSpan="1" role="columnheader"></th>
                      )}
                      {data.currentCert.isBackgroundCheckRequired ? (
                        <th
                          className="text-center "
                          colSpan="1"
                          role="columnheader"
                        >
                          Background Check Completed
                        </th>
                      ) : (
                        <th colSpan="1" role="columnheader"></th>
                      )}
                      {data.currentCert.isTestRequired ? (
                        <th
                          className="text-center "
                          colSpan="1"
                          role="columnheader"
                        >
                          Test Completed
                        </th>
                      ) : (
                        <th colSpan="1" role="columnheader"></th>
                      )}
                      {data.currentCert.isFitnessTestRequired ? (
                        <th
                          className="text-center "
                          colSpan="1"
                          role="columnheader"
                        >
                          Fitness Test Completed
                        </th>
                      ) : (
                        <th colSpan="1" role="columnheader"></th>
                      )}
                      {data.currentCert.isClinicRequired ? (
                        <th
                          className="text-center "
                          colSpan="1"
                          role="columnheader"
                        >
                          Clinic Attended
                        </th>
                      ) : (
                        <th colSpan="1" role="columnheader"></th>
                      )}
                      <th colSpan="1" role="columnheader" />
                    </tr>
                  </thead>

                  {data.certArrayComponents}
                </Table>
                <Col className=" d-flex center-text  justify-content-center align-items-center pb-2">
                  <Pagination
                    locale={locale}
                    onChange={onPaginationChange}
                    current={data.current}
                    total={data.total}
                    pageSize={data.pageSize}
                  />
                </Col>
              </div>
            </CardBody>
          </Card>
        </Col>
      </Row>
    </React.Fragment>
  );
}
export default CertificationResults;
